﻿QUnit.module("TypeScript - Interfaces");
QUnit.test("Simple field and property", function (assert) {
    var instance = new Interfaces.Class1();

    TestInstance1(assert, instance);
});

QUnit.test("Four methods", function (assert) {
    var instance = new Interfaces.Class2();

    TestInstance1(assert, instance);

    TestInstance2(assert, instance);
});

QUnit.test("Fifth method through Interface3", function (assert) {
    var instance = new Interfaces.Class3();

    TestInstance1(assert, instance);

    TestInstance2(assert, instance);

    TestInstance3(assert, instance);
});

QUnit.test("Out and reference parameters", function (assert) {
    var instance = new Interfaces.Class4();
    TestInstance4(assert, instance);
});

QUnit.test("Property setter and getter with same names as methods", function (assert) {
    var instance = new Interfaces.Class6();
    TestInstance6(assert, instance);
});

function TestInstance1(assert, instance) {
    var interface1 = instance;

    assert.deepEqual(interface1.getProperty(), 100, "Interface1 Property getter");
    assert.deepEqual(instance.field, 200, "Class1 Field initial value");

    interface1.setProperty(300);
    assert.deepEqual(instance.getProperty(), 300, "Class1 Property setter");
}

function TestInstance2(assert, instance) {
    var interface2 = instance;

    interface2.method1();
    assert.deepEqual(interface2.getProperty(), 2, "Method1() through Property");
    assert.deepEqual(instance.field, 1, "Method1() through Fileld");

    interface2.method2("1234567");
    assert.deepEqual(instance.field, 7, "Method2() through Field");

    assert.deepEqual(interface2.method3(), instance.field, "Method3 through Field");

    assert.ok(interface2.method4(instance), "Method4 through return result");
    assert.deepEqual(instance.field, interface2.getProperty(), "Method4 through Field");
}

function TestInstance3(assert, instance) {
    var interface3 = instance;

    var interface2 = interface3.method5(interface3);
    assert.deepEqual(interface3.getProperty(), interface2.getProperty(), "Method5 through Property");

    var instance1 = instance;
    assert.deepEqual(instance1.field, instance.field, "Method5 through Field");
}

function TestInstance4(assert, instance) {
    var interface4 = instance;

    var boolValue = false;
    var boolRef = { v: boolValue };

    instance.method6(boolRef);
    assert.deepEqual(boolRef.v, true, "Method6() out bool parameter");

    boolRef.v = false;
    instance.method7(1, boolRef);
    assert.deepEqual(boolRef.v, true, "Method7() int, out bool parameter");

    var stringValue = "ABC_";
    var stringRef = { v: stringValue };
    instance.method8(stringRef);
    assert.deepEqual(stringRef.v, "ABC_Method8", "Method8() ref string parameter");

    stringRef.v = "ABC_";
    instance.method9(1, stringRef);
    assert.deepEqual(stringRef.v, "ABC_1", "Method9() int, ref string parameter");

    stringRef.v = "ABC_";
    boolRef.v = false;
    instance.method10(2, boolRef, stringRef);
    assert.deepEqual(boolRef.v, true, "Method10() int, ref bool, out bool parameter");
    assert.deepEqual(stringRef.v, "ABC_2", "Method10() int, ref bool, ref string parameter");
}

function TestInstance6(assert, instance) {
    var interface6 = instance;

    interface6.setProperty$3(1);
    assert.deepEqual(instance.getProperty$3(), 1, "Property getter and setter");

    interface6.setProperty$1("12");
    assert.deepEqual(instance.getMethodProperty(), 2, "setProperty$1(string) and MethodProperty");
    assert.deepEqual(instance.getProperty(), 2, "setPropert$1(string) and getProperty");
    assert.notDeepEqual(instance.getProperty(), instance.getProperty$3(), "getProperty and getProperty$3");

    interface6.setProperty(3);
    assert.deepEqual(instance.getMethodProperty(), 3, "setProperty(int) and MethodProperty");
    assert.deepEqual(instance.getProperty(), 3, "setPropert$1(int) and getProperty");
    assert.notDeepEqual(instance.getProperty$3(), instance.getProperty(), "getProperty$3 and getProperty");

    var interface61 = instance;
    var interface62 = instance;
}
