using System.Collections;

namespace FuncExecutor {
    public interface FE_IFunction {
        IEnumerator IGetFunction(IFunctionExecutor executor);
        bool IGetIsAsyn();
    }
}