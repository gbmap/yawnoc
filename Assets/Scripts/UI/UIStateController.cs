using Frictionless;
using Messages.Gameplay;
using UnityEngine;

public class UIStateController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Gameplay;

    void OnEnable()
    {
        MessageRouter.AddHandler<Messages.Gameplay.ChangeState>(Cb_ChangeState);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<Messages.Gameplay.ChangeState>(Cb_ChangeState);
    }

    private void Cb_ChangeState(ChangeState obj)
    {
        MainMenu.SetActive(false);
        Gameplay.SetActive(false);

        switch (obj.State)
        {
            case State.Gameplay: Gameplay.SetActive(true);  break;
            case State.Menu: MainMenu.SetActive(true);      break;
        }

        return;
    }
}
