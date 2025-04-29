using System.Collections;
using UnityEngine;

public class PlayerExample : BasePlayerController, IAimable, IMoveable, IAttackable
{
    public enum MoveMode { Move2D, Move3D, Rotate }
    public enum AttackMode { Shoot, Explode, Dash }

    [Header("Movement Settings")]
    public MoveMode currentMoveMode;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;

    [Header("Attack Settings")]
    public AttackMode currentAttackMode;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionHeight = 0.1f;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;

    public Vector2 Position
    {
        get => _aimPosition;
        set
        {
            _aimPosition = value;
            Debug.Log("Aim from " + name + ": " + _aimPosition);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Child Awake");
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("Child Start");
    }

    public void Move(Vector2 direction)
    {
        switch (currentMoveMode)
        {
            case MoveMode.Move2D:
                Move2D(direction);
                break;
            case MoveMode.Move3D:
                Move3D(direction);
                break;
            case MoveMode.Rotate:
                Rotate(direction);
                break;
        }
    }

    private void Move2D(Vector2 dir)
    {
        Vector3 delta = new Vector3(dir.x, dir.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += delta;
        Debug.Log("2D Move from " + name + ": " + dir);
    }

    private void Move3D(Vector2 dir)
    {
        Vector3 delta = new Vector3(dir.x, 0f, dir.y) * moveSpeed * Time.deltaTime;
        transform.position += delta;
        Debug.Log("3D Move from " + name + ": " + dir);
    }

    private void Rotate(Vector2 dir)
    {
        float yaw = dir.x * turnSpeed * Time.deltaTime;
        float pitch = -dir.y * turnSpeed * Time.deltaTime;
        transform.Rotate(pitch, yaw, 0f, Space.Self);
        Debug.Log("Rotate from " + name + ": " + dir);
    }

    public void Attack(Vector2 aimPos)
    {
        switch (currentAttackMode)
        {
            case AttackMode.Shoot:
                Shoot(aimPos);
                break;
            case AttackMode.Explode:
                Explode(aimPos);
                break;
            case AttackMode.Dash:
                Dash(aimPos);
                break;
        }
    }

    private void Shoot(Vector2 aimPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f));
        Vector3 dir = (world - transform.position).normalized;
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(dir));
        if (b.TryGetComponent<Rigidbody>(out var rb))
            rb.linearVelocity = dir * bulletSpeed;
        Debug.Log("Shoot from " + name + " toward " + world);
    }

    private void Explode(Vector2 aimPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f));
        Vector3 pos = new Vector3(world.x, explosionHeight, world.z);
        Instantiate(explosionPrefab, pos, Quaternion.identity);
        Debug.Log("Explode from " + name + " at " + pos);
    }

    private void Dash(Vector2 aimPos)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f));
        Vector3 dir = (world - transform.position).normalized;
        StopAllCoroutines();
        StartCoroutine(DashRoutine(dir));
    }

    private IEnumerator DashRoutine(Vector3 dir)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + dir * dashDistance;
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        Debug.Log("Dash complete from " + name);
    }
}