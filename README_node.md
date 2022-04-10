[FunctionExecutorはこちら](/README.md)
# FunctionExecutor_Node

GameObjectに命令を付与し、非同期的に逐次実行する。

<!--# DEMO

-->


# Requirement

* UnityEngine
* System
* System.Collections

# Usage

```
GameObject target;
bool moveToOne = false;

FunctionExecutor_Node executor = target.ComponentFunctionExecutor_Node(); //Add or GetComponent
executor.SetNode(0, (true, 1, () => moveToOne), (false, 0, () => true))
    .SetFunction(
        // Called when node index is 0.
        // new FE_Function1(),
        // new FE_Function2(),
        // ...
    )
    .SetNode(1, (true, 0, () => !moveToOne), (false, 1, () => true))
    .SetFunction(
        // Called when node index is 1.
        // new FE_Function1(),
        // new FE_Function2(),
        // ...
    ).BeginAction(0) // begin node index.
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

## Public Function
```
FunctionExecutor_Node SetNode(int index, params (bool, int, Func<bool>)[] nodeTransitions)
FunctionExecutor_Node SetNode(int index, NodeTransition[] nodeTransitions = null, NodeTransition[] nodeForcedTransitions = null)
FunctionExecutor_Node SetFunction(params FE_IFunction[] functions)
Coroutine BeginAction(int index = 0)
MonoBehaviour IGetMonoBehaviour()
```
### NodeTransition (struct)
```
NodeTransition(int nextNodeIndex, Func<bool> condition)
int? GetTransitionOrder()
```

# Note

* bool asynをtrueにすると、前の命令の終了を待たずに実行されます。
* 新たな命令を追加したい場合には、F_ActionやF_Coroutineを使うか、FE_IFunctionを継承したクラスを作ってください。
* このプログラムを敵の操作に使ったり、TransformやGameObjectに頻繁にアクセスする場合にはFunctionExecutorの上位互換のEntityActionConを使ってください。

# License

"FunctionExecutor" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
