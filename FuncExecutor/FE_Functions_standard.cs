using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System;

namespace FuncExecutor {
    //===== 基本Function =====
    public struct F_WaitForSeconds : FE_IFunction {
        private float time;
        public F_WaitForSeconds(float time) {
            this.time = time;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            yield return new WaitForSeconds(time);
        }
        public bool IGetIsAsyn() => false;
    }

    public struct F_WaitUntil : FE_IFunction {
        private Func<bool> condition;
        public F_WaitUntil(Func<bool> condition) {
            this.condition = condition;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            yield return new WaitUntil(condition);
        }
        public bool IGetIsAsyn() => false;
    }

    public struct F_Destroy : FE_IFunction {
        private GameObject gameObject;
        private System.Action action;
        /// <summary>
        /// オブジェクトを破壊する。
        /// </summary>
        /// <param name="gameObject">nullなら命令を実行したオブジェクトを破壊する。</param>
        /// <param name="action">破壊時に実行するコマンド</param>
        public F_Destroy(GameObject gameObject = null, System.Action action = null) {
            this.gameObject = gameObject;
            this.action = action;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            if (action != null) action();
            yield return null;
            if (gameObject == null) gameObject = executor.IGetMonoBehaviour().gameObject;
            MonoBehaviour.Destroy(gameObject);
        }
        public bool IGetIsAsyn() => false;
    }

    public struct F_DebugLog : FE_IFunction {
        private string message;
        public F_DebugLog(string message) {
            this.message = message;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            Debug.Log(message, executor.IGetMonoBehaviour().gameObject);
            yield return null;
        }
        public bool IGetIsAsyn() => false;
    }

    //===== 特殊Function =====
    public struct F_ChainFunction : FE_IFunction { //functionグループ
        private FE_IFunction[] functions;
        private bool asyn;
        public F_ChainFunction(bool asyn, params FE_IFunction[] functions) {
            this.asyn = asyn;
            this.functions = MergeArrayClass.MergeArray(functions);
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            return FunctionExecutor.FunctionsExecute(executor, this.functions);
        }
        public bool IGetIsAsyn() => asyn;
    }

    /*public struct F_MultiFunction : FE_IFunction {
        private FE_IFunction[] functions;
        private bool asyn;
        public F_MultiFunction(bool asyn, params FE_IFunction[] functions) {
            this.asyn = asyn;
            this.functions = MergeArrayClass.MergeArray(functions);
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {

        }
        public bool IGetIsAsyn() => asyn;
        private class FunctionChecker {
            private FE_IFunction function;
            private int runState = 0;
        }
    }*/

    public struct F_LoopFunction : FE_IFunction {
        enum LoopType { Condition, Count, Both }
        private LoopType type;
        private Func<bool> condition;
        private int count;
        private bool and;
        private FE_IFunction[] functions;
        private bool asyn;
        public F_LoopFunction(bool asyn, Func<bool> condition, params FE_IFunction[] functions) : this(asyn, functions) {
            this.type = LoopType.Condition;
            this.condition = condition;
        }
        public F_LoopFunction(bool asyn, int count, params FE_IFunction[] functions) : this(asyn, functions) {
            this.type = LoopType.Count;
            this.count = count;
        }
        public F_LoopFunction(bool asyn, Func<bool> condition, int count, bool and, params FE_IFunction[] functions) : this(asyn, functions) {
            this.type = LoopType.Both;
            this.condition = condition;
            this.count = count;
            this.and = and;
        }
        private F_LoopFunction(bool asyn, params FE_IFunction[] functions) {
            this.functions = MergeArrayClass.MergeArray(functions);
            this.asyn = asyn;
            this.type = default;
            this.condition = default;
            this.count = default;
            this.and = default;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            int _count = count;
            while (LoopCondition(_count)) {
                yield return FunctionExecutor.FunctionsExecute(executor, this.functions);
                _count = Mathf.Max(0, _count - 1);
            }
        }
        private bool LoopCondition(float count) {
            bool t1 = type == LoopType.Condition && condition();
            bool t2 = type == LoopType.Count && count > 0;
            bool t3 = type == LoopType.Both && (and ? condition() && count > 0 : condition() || count > 0);
            return t1 || t2 || t3;
        }
        public bool IGetIsAsyn() => this.asyn;
    }

    public struct F_Action : FE_IFunction {
        private Action action;
        public F_Action(Action action) {
            this.action = action;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) {
            action();
            return null;
        }
        public bool IGetIsAsyn() => false;
    }

    public class F_Coroutine : FE_IFunction {
        private Func<IEnumerator> enumerator;
        private bool asyn;
        public F_Coroutine(bool asyn, Func<IEnumerator> enumerator) {
            this.enumerator = enumerator;
            this.asyn = asyn;
        }
        public IEnumerator IGetFunction(IFunctionExecutor executor) => this.enumerator();
        public bool IGetIsAsyn() => this.asyn;
    }
}
