[FunctionExecutor_Nodeはこちら](/README_node.md)

# FunctionExecutor

GameObjectに命令を付与し、非同期的に逐次実行する。
[EntityActionCon](https://github.com/kuritaro1122/EntityActionCon)の基底プログラムです。

<!--# DEMO

-->


# Requirement

* UnityEngine
* System
* System.Collections

# Usage

```
GameObject target;
FunctionExecutor executor = target.ComponentFunctionExecutor(); //Add or GetComponent
executor.Set(
    // Write the function you want to execute.
    // new FE_Function1(),
    // new FE_Function2(),
    // ...
).BeginAction();
```
## FE_Functions
```
F_WaitForSeconds(float time)
F_WaitUntil(System.Func<bool> condition)
F_Destroy(GameObject gameObject = null)
F_DebugLog(string message)
F_Action(System.Action action)
F_Action(System.Action<IFunctionExecutor> action)
F_Coroutine(bool asyn, System.Func<IEnumerator> enumerator)
F_Coroutine(bool asyn, System.Func<IFunctionExecutor, IEnumerator> enumerator)

F_ChainFunction(bool asyn, params FE_IFunction[] functions)
F_LoopFunction(bool asyn, Func<bool> condition, params FE_IFunction[] functions)
F_LoopFunction(bool asyn, int count, params FE_IFunction[] functions)
F_LoopFunction(bool asyn, Func<bool> condition, int count, bool and, params FE_IFunction[] functions)
```
## FE_IFunction (Interface)
```
IEnumerator IGetFunction(IFunctionExecutor executor)
bool IGetAsyn()
```

# Contains

<!--## Inspector

-->

## Public Variable
```
bool Running { get; }
```
## Public Function
```
FunctionExecutor Set(params FE_IFunction[] functions)
FunctionExecutor ResetAll()
Coroutine BeginAction()
```

# Note

* bool asynをtrueにすると、前の命令の終了を待たずに実行されます。
* 新たな命令を追加したい場合には、F_ActionやF_Coroutineを使うか、FE_IFunctionを継承したクラスを作ってください。

# License

"FunctionExecutor" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
