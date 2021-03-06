using Bridge.Contract;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridge.Translator
{
    public class SwitchBlock : AbstractEmitterBlock
    {
        public SwitchBlock(IEmitter emitter, SwitchStatement switchStatement)
            : base(emitter, switchStatement)
        {
            this.Emitter = emitter;
            this.SwitchStatement = switchStatement;
        }

        public SwitchBlock(IEmitter emitter, SwitchSection switchSection)
            : base(emitter, switchSection)
        {
            this.Emitter = emitter;
            this.SwitchSection = switchSection;
        }

        public SwitchBlock(IEmitter emitter, CaseLabel caseLabel)
            : base(emitter, caseLabel)
        {
            this.Emitter = emitter;
            this.CaseLabel = caseLabel;
        }

        public SwitchStatement SwitchStatement
        {
            get;
            set;
        }

        public SwitchSection SwitchSection
        {
            get;
            set;
        }

        public CaseLabel CaseLabel
        {
            get;
            set;
        }

        public SwitchStatement ParentAsyncSwitch
        {
            get;
            set;
        }

        protected override void DoEmit()
        {
            if (this.SwitchStatement != null)
            {
                var awaiters = this.Emitter.IsAsync ? this.GetAwaiters(this.SwitchStatement) : null;

                if (awaiters != null && awaiters.Length > 0)
                {
                    this.VisitAsyncSwitchStatement();
                }
                else
                {
                    this.VisitSwitchStatement();
                }
            }
            else if (this.SwitchSection != null)
            {
                if (this.Emitter.AsyncSwitch != null)
                {
                    throw new EmitterException(this.SwitchSection, "Async switch section must be handled by VisitAsyncSwitchStatement method");
                }
                else
                {
                    this.VisitSwitchSection();
                }
            }
            else
            {
                if (this.Emitter.AsyncSwitch != null)
                {
                    throw new EmitterException(this.CaseLabel, "Async case label must be handled by VisitAsyncSwitchStatement method");
                }
                else
                {
                    this.VisitCaseLabel();
                }
            }
        }

        protected void VisitAsyncSwitchStatement()
        {
            SwitchStatement switchStatement = this.SwitchStatement;
            this.ParentAsyncSwitch = this.Emitter.AsyncSwitch;
            this.Emitter.AsyncSwitch = switchStatement;

            this.WriteAwaiters(switchStatement.Expression);

            var oldValue = this.Emitter.ReplaceAwaiterByVar;
            this.Emitter.ReplaceAwaiterByVar = true;
            string key = null;

            if (switchStatement.Expression is IdentifierExpression)
            {
                var oldBuilder = this.Emitter.Output;
                this.Emitter.Output = new StringBuilder();

                switchStatement.Expression.AcceptVisitor(this.Emitter);
                key = this.Emitter.Output.ToString().Trim();

                this.Emitter.Output = oldBuilder;
            }
            else
            {
                key = this.AddLocal(this.GetTempVarName(), AstType.Null);
                this.Write(key);
                this.Write(" = ");
                switchStatement.Expression.AcceptVisitor(this.Emitter);
                this.WriteSemiColon();
                this.WriteNewLine();
            }

            this.Emitter.ReplaceAwaiterByVar = oldValue;

            var list = switchStatement.SwitchSections.ToList();
            list.Sort((s1, s2) =>
            {
                var lbl = s1.CaseLabels.FirstOrDefault(l => l.Expression.IsNull);

                if (lbl != null)
                {
                    return 1;
                }

                lbl = s2.CaseLabels.FirstOrDefault(l => l.Expression.IsNull);

                if (lbl != null)
                {
                    return -1;
                }

                return 0;
            });

            var jumpStatements = this.Emitter.JumpStatements;
            this.Emitter.JumpStatements = new List<IJumpInfo>();
            bool writeElse = false;
            var thisStep = this.Emitter.AsyncBlock.Steps.Last();

            foreach (var switchSection in list)
            {
                this.VisitAsyncSwitchSection(switchSection, writeElse, key);
                writeElse = true;
            }

            var nextStep = this.Emitter.AsyncBlock.AddAsyncStep();
            thisStep.JumpToStep = nextStep.Step;

            if (this.Emitter.JumpStatements.Count > 0)
            {
                this.Emitter.JumpStatements.Sort((j1, j2) => -j1.Position.CompareTo(j2.Position));
                foreach (var jump in this.Emitter.JumpStatements)
                {
                    if (jump.Break)
                    {
                        jump.Output.Insert(jump.Position, nextStep.Step);
                    }
                    else if (jumpStatements != null)
                    {
                        jumpStatements.Add(jump);
                    }
                }
            }

            this.Emitter.JumpStatements = jumpStatements;
            this.Emitter.AsyncSwitch = this.ParentAsyncSwitch;
        }

        protected void VisitAsyncSwitchSection(SwitchSection switchSection, bool writeElse, string switchKey)
        {
            var list = switchSection.CaseLabels.ToList();

            list.Sort((l1, l2) =>
            {
                if (l1.Expression.IsNull)
                {
                    return 1;
                }

                if (l2.Expression.IsNull)
                {
                    return -1;
                }

                return 0;
            });

            if (writeElse)
            {
                this.WriteElse();
            }

            if (list.Any(l => l.Expression.IsNull))
            {
                if (!writeElse)
                {
                    this.WriteElse();
                }
            }
            else
            {
                this.WriteIf();
                this.WriteOpenParentheses();

                var oldValue = this.Emitter.ReplaceAwaiterByVar;
                this.Emitter.ReplaceAwaiterByVar = true;
                bool writeOr = false;

                foreach (var label in list)
                {
                    if (writeOr)
                    {
                        this.WriteSpace();
                        this.Write("||");
                        this.WriteSpace();
                    }

                    this.Write(switchKey + " === ");
                    label.Expression.AcceptVisitor(this.Emitter);

                    writeOr = true;
                }

                this.WriteCloseParentheses();
                this.Emitter.ReplaceAwaiterByVar = oldValue;
            }

            if (switchSection.Statements.Count() == 1 && switchSection.Statements.First() is BlockStatement)
            {
                this.Emitter.IgnoreBlock = switchSection.Statements.First();
            }

            int startCount = this.Emitter.AsyncBlock.Steps.Count;
            IAsyncStep thisStep = null;
            this.WriteSpace();
            this.BeginBlock();
            this.Write("$step = " + this.Emitter.AsyncBlock.Step + ";");
            this.WriteNewLine();
            this.Write("continue;");
            var writer = this.SaveWriter();
            var bodyStep = this.Emitter.AsyncBlock.AddAsyncStep();

            switchSection.Statements.AcceptVisitor(this.Emitter);

            if (this.Emitter.AsyncBlock.Steps.Count > startCount)
            {
                thisStep = this.Emitter.AsyncBlock.Steps.Last();
            }

            if (this.RestoreWriter(writer) && !this.IsOnlyWhitespaceOnPenultimateLine(true))
            {
                this.WriteNewLine();
            }

            this.EndBlock();
            this.WriteNewLine();
        }

        protected void VisitSwitchStatement()
        {
            SwitchStatement switchStatement = this.SwitchStatement;

            this.WriteSwitch();
            this.WriteOpenParentheses();

            switchStatement.Expression.AcceptVisitor(this.Emitter);

            this.WriteCloseParentheses();
            this.WriteSpace();

            this.BeginBlock();
            switchStatement.SwitchSections.ToList().ForEach(s => s.AcceptVisitor(this.Emitter));
            this.EndBlock();
            this.WriteNewLine();
        }

        protected void VisitSwitchSection()
        {
            SwitchSection switchSection = this.SwitchSection;

            switchSection.CaseLabels.ToList().ForEach(l => l.AcceptVisitor(this.Emitter));
            this.Indent();
            switchSection.Statements.ToList().ForEach(s => s.AcceptVisitor(this.Emitter));
            this.Outdent();
        }

        protected void VisitCaseLabel()
        {
            CaseLabel caseLabel = this.CaseLabel;

            if (caseLabel.Expression.IsNull)
            {
                this.Write("default");
            }
            else
            {
                this.Write("case ");
                caseLabel.Expression.AcceptVisitor(this.Emitter);
            }

            this.WriteColon();
            this.WriteNewLine();
        }
    }
}
