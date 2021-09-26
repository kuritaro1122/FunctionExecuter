using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuncExecutor;

internal class FunctionExecutorTester : MonoBehaviour
{
    [SerializeField] GameObject entity;
    [SerializeField] GameObject entity2;

    // Start is called before the first frame update
    void Start()
    {
        entity.ComponentFunctionExecutor()
            .Set()
            .Set()
            .BeginAction();

        entity2.ComponentFunctionExecutor_Node()
            .SetNode(0, (false, 1, () => true))
            .SetFunction(new F_DebugLog("node0"))
            .SetFunction(new F_WaitForSeconds(1f))
            //.SetFunction(new F_Destroy())
            .SetNode(1, (false, 0, () => true))
            .SetFunction(
                new F_DebugLog("node1"),
                new F_WaitForSeconds(1f)
                )
            .BeginAction(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
