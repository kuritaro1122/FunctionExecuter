using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System;

namespace FuncExecutor {
    public class FunctionExecutor_Node : MonoBehaviour, IFunctionExecutor {
        private FunctionNode[] functionNodes;
        private int nodeIndex;
        public int GetNodeIndex() => this.nodeIndex;

        private FunctionNode GetNode() {
            if (functionNodes != null && functionNodes.Length - 1 >= nodeIndex)
                return functionNodes[nodeIndex];
            else return null;
        }

        public FunctionExecutor_Node ResetAll() {
            KillCoroutine(ref mainCoroutine);
            KillCoroutine(ref functionCoroutine);
            this.functionNodes = null;
            return this;
        }
        /// <summary>
        /// ノードの作成 and 選択
        /// </summary>
        /// <param name="index">node index</param>
        /// <param name="nodeTransitions">Node's transitions (during execution, next index, conditon)</param>
        public FunctionExecutor_Node SetNode(int index, params (bool, int, Func<bool>)[] nodeTransitions) {
            NodeTransition[] _nodeTransitions = null;
            NodeTransition[] _nodeForcedTransitions = null;
            foreach (var nt in nodeTransitions) {
                (bool forced, int nextIndex, Func<bool> condition) = nt;
                NodeTransition _nodeTransition = new NodeTransition(nextIndex, condition);
                if (forced) _nodeForcedTransitions = MergeArrayClass.MergeArray(_nodeForcedTransitions, _nodeTransition);
                else _nodeTransitions = MergeArrayClass.MergeArray(_nodeTransitions, _nodeTransition);
            }
            return SetNode(index, _nodeTransitions, _nodeForcedTransitions);
        }
        /// <summary>
        /// ノードの作成 and 選択
        /// </summary>
        /// <param name="index">node index</param>
        /// <param name="nodeTransitions"></param>
        /// <param name="nodeForcedTransitions"></param>
        /// <returns></returns>
        public FunctionExecutor_Node SetNode(int index, NodeTransition[] nodeTransitions = null, NodeTransition[] nodeForcedTransitions = null) {
            //必要な長さのActionNodeを確保
            int overflow = Mathf.Max(index - ((this.functionNodes?.Length ?? 0) - 1), 0);
            if (this.functionNodes == null)
                this.functionNodes = new FunctionNode[overflow];
            else
                this.functionNodes = MergeArrayClass.MergeArray(this.functionNodes, new FunctionNode[overflow]);
            //ActionNodeのセット
            this.functionNodes[index] = new FunctionNode(nodeTransitions, nodeForcedTransitions);
            this.nodeIndex = index;
            return this;
        }

        /// <summary>
        /// 選択されたノードにfunctionを追加する。
        /// </summary>
        public FunctionExecutor_Node SetFunction(params FE_IFunction[] functions) {
            GetNode().Set(functions);
            return this;
        }

        /// <summary>
        /// 設定した命令を実行
        /// </summary>
        /// <param name="index"></param>
        /// <returns>実行したコルーチン</returns>
        public Coroutine BeginAction(int index = 0) { //重複の可能性
            this.nodeIndex = index;
            mainCoroutine = MainCoroutine();
            return StartCoroutine(mainCoroutine);
        }
        /*
        /// <summary>
        /// 設定した命令を実行(IEnumerator
        /// </summary>
        public IEnumerator BeginAction_coroutine() { //後回し
            return null;
        }*/

        private IEnumerator mainCoroutine = null;
        private IEnumerator functionCoroutine = null;

        private IEnumerator MainCoroutine() {
            while (true) {
                if (this.nodeIndex < 0) break;
                FunctionNode action = GetNode();
                if (action == null) {
                    Debug.LogError("EntityActionCon_Node/ActionNodes is null.\nindex => " + nodeIndex, this);
                    break;
                }
                functionCoroutine = Function();
                StartCoroutine(functionCoroutine);

                IEnumerator Function() {
                    yield return action.GetFunctionCoroutine(this);
                    int? nextIndex = action.GetTransitionOrderIndex();
                    this.nodeIndex = nextIndex ?? this.nodeIndex;
                    this.nodeIndex = nextIndex ?? -1;
                    functionCoroutine = null;
                }
                //StopAllCoroutines();
                yield return new WaitUntil(() => functionCoroutine == null);
            }
        }

        void Update() {
            //強制ノード遷移処理
            if (this.nodeIndex < 0) return;
            if (GetNode() != null) {
                int? nextIndex = GetNode().GetForcedTransitionOrderIndex();
                if (nextIndex != null) {
                    this.nodeIndex = nextIndex ?? 0;
                    //StopCoroutine(functionCoroutine);
                    StopAllCoroutines();
                    functionCoroutine = null;
                    BeginAction(this.nodeIndex);
                }
            }
        }

        //ノード遷移

        private void KillCoroutine(ref IEnumerator enumerator) {
            if (enumerator == null) return;
            StopCoroutine(enumerator);
            enumerator = null;
        }

        private class FunctionNode {
            private FE_IFunction[] functions;
            private NodeTransition[] nodeTransitions;
            private NodeTransition[] nodeForcedTransitions;
            public FunctionNode(NodeTransition[] nodeTransitions = null, NodeTransition[] nodeForcedTransitions = null, FE_IFunction[] functions = null) {
                this.functions = MergeArrayClass.MergeArray(this.functions, functions);
                this.nodeTransitions = MergeArrayClass.MergeArray(null, nodeTransitions);
                this.nodeForcedTransitions = MergeArrayClass.MergeArray(null, nodeForcedTransitions);
            }
            public void Set(NodeTransition[] nodeTransitions = null, NodeTransition[] nodeForcedTransitions = null) {
                this.nodeTransitions = MergeArrayClass.MergeArray(this.nodeTransitions, nodeTransitions);
                this.nodeForcedTransitions = MergeArrayClass.MergeArray(this.nodeForcedTransitions, nodeForcedTransitions);
            }
            public void Set(params FE_IFunction[] functions) {
                this.functions = MergeArrayClass.MergeArray(this.functions, functions);
            }
            public IEnumerator GetFunctionCoroutine(IFunctionExecutor executor) {
                return FunctionExecutor.FunctionsExecute(executor, this.functions);
            }
            public int? GetTransitionOrderIndex() => GetOrderIndex(nodeTransitions);
            public int? GetForcedTransitionOrderIndex() => GetOrderIndex(nodeForcedTransitions);
            private int? GetOrderIndex(NodeTransition[] nts) {
                foreach (var nt in nts) {
                    int? orderIndex = nt.GetTransitionOrder();
                    if (orderIndex != null) return orderIndex;
                }
                return null;
            }
        }
        public struct NodeTransition {
            private Func<bool> condition;
            private int nextNodeIndex;
            public NodeTransition(int nextNodeIndex, Func<bool> condition) {
                this.nextNodeIndex = nextNodeIndex;
                this.condition = condition;
            }
            public int? GetTransitionOrder() {
                if (condition()) return nextNodeIndex;
                else return null;
            }
        }

        public MonoBehaviour IGetMonoBehaviour() => this;
    }

    public static class AddComponenter_FE_N {
        public static FunctionExecutor_Node ComponentFunctionExecutor_Node(this GameObject self) {
            return self.ComponentCheck<FunctionExecutor_Node>();
        }
    }
}