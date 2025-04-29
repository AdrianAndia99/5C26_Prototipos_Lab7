using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandler : MonoBehaviour
{
    [SerializeField] private GameObject PlayerController;

    private IAimable _characterAim;
    private IMoveable _characterMovement;
    private IAttackable _characterAttack;

    private void Awake()
    {
        _characterAim = PlayerController.GetComponent<IAimable>();
        _characterMovement = PlayerController.GetComponent<IMoveable>();
        _characterAttack = PlayerController.GetComponent<IAttackable>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            _characterMovement.Move(context.ReadValue<Vector2>());
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        _characterAim.Position = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _characterAttack.Attack(_characterAim.Position);
        }
    }
}