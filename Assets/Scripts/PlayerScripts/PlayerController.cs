using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IUpdate
{
    PlayerModel model;
    FSM<PlayerEnums> fsm;
    List<PlayerStateBase<PlayerEnums>> states;

    public void InitializeFSM()
    {
        //el player state base se crea para no tener que hacer siempre
        //la misma funci�n y no tener que pasar siempre los mismos componentes
        //que son el model y el fsm.

        fsm = new FSM<PlayerEnums>();
        states = new List<PlayerStateBase<PlayerEnums>>();

        var idle = new PlayerStateIdle<PlayerEnums>(PlayerEnums.Running);
        var move = new PlayerStateMove<PlayerEnums>(PlayerEnums.Idle);

        //al no tener que llamar a la misma funci�n siempre
        //metemos todos los estados en una lista
        states.Add(idle);
        states.Add(move);

        //recorremos esa lista e inicializamos los estados
        for (int i = 0; i < states.Count; i++)
        {
            //as�. con este for se inicializan todos los estados.
            states[i].InitializeState(model, fsm);
        }
        states = null;

        //si al idle le paso este input "playerEnums.Running" va a "move"
        idle.AddTransition(PlayerEnums.Running, move);

        //si al idle le paso este input "playerEnums.Idle" va a "idle"
        move.AddTransition(PlayerEnums.Idle, idle);

        fsm.SetInit(idle);
    }

    private void Awake()
    {
        model = GetComponent<PlayerModel>();
        InitializeFSM();
    }

    private void Start()
    {
        GameManager.instance.updateManager.AddToGameplayUpdate(this);
    }

    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.instance.updateManager.RemoveToGameplayUpdate(this);
    }

    public void DoUpdate()
    {
        fsm.OnUpdate();
    }
}
