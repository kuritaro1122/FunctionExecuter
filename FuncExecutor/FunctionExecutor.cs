﻿using System.Collections;
using UnityEngine;

namespace FuncExecutor {
    public class FunctionExecutor : MonoBehaviour, IFunctionExecutor {
        private FE_IFunction[] functions;
        public bool Running { get; private set; } = false;
        /// <summary>
        /// 命令を初期化
        /// </summary>
        public FunctionExecutor ResetAll() {
            this.functions = null;
            this.StopAllCoroutines();
            return this;
        }
        /// <summary>
        /// 命令を設定 or 追加
        /// </summary>
        public FunctionExecutor Set(params FE_IFunction[] functions) {
            this.functions = MergeArrayClass.MergeArray(this.functions, functions);
            return this;
        }
        /// <summary>
        /// 設定した命令を実行
        /// </summary>
        public Coroutine BeginAction() { //重複の可能性
            if (this.Running) {
                UnityEngine.Debug.LogWarning("FunctionExecutor/ already execute function.", this);
                return null;
            }
            return StartCoroutine(MainCoroutine());
        }

        private IEnumerator MainCoroutine() {
            this.Running = true;
            yield return FunctionsExecute(this, this.functions);
            this.Running = false;
        }
        public MonoBehaviour IGetMonoBehaviour() => this;

        public static IEnumerator FunctionsExecute(IFunctionExecutor executor, params FE_IFunction[] functions) {
            foreach (var function in functions) {
                IEnumerator enumerator = function.IGetFunction(executor);
                if (function.IGetIsAsyn())
                    executor.IGetMonoBehaviour().StartCoroutine(enumerator);
                else yield return enumerator;
            }
            //if (functions == null) yield return null;
        }
    }

    public static class AddComponenter {
        public static T ComponentCheck<T>(this GameObject self) where T : MonoBehaviour {
            T component = self.GetComponent<T>();
            if (component == null) {
                component = self.AddComponent<T>();
                Debug.Log(component.GetType() + "をコンポーネントしました。", self);
            }
            return component;
        }
    }
    public static class AddComponenter_FE {
        public static FunctionExecutor ComponentFunctionExecutor(this GameObject self) {
            return self.ComponentCheck<FunctionExecutor>();
        }
    }
}