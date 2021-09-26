using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

namespace FuncExecutor {
    public interface FE_IFunction {
        IEnumerator IGetFunction(IFunctionExecutor executor);
        bool IGetIsAsyn(); //いらなくね？
    }
}