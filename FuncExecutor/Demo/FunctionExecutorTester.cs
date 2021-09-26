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
            .SetNode(0)
            .SetFunction(new F_WaitForSeconds(4f))
            .SetFunction(new F_DebugLog("test"))
            .SetFunction(new F_Destroy())
            .BeginAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
