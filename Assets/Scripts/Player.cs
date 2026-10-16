using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DirectionForceApplier rotationDirection;
    private readonly float rotationSpeed = 800f;
    public float totalRotation;
    public bool isRotating;
    public Vector3 pivot, axis, scale;

    // Stan toczenia - każda klatka animacji liczona jest od pozycji startowej,
    // a na końcu ruchu blok ustawiany jest dokładnie w pozycji docelowej,
    // dzięki czemu błędy float nie kumulują się między ruchami.
    private Vector3 rollStartPosition;
    private Quaternion rollStartRotation;
    private Vector3 rollTargetPosition;
    private float rollAngleSign;

    [Header("Player Management")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider boxCollider;
    public bool canMove;
    private bool isFalling;
    public bool IsFalling => isFalling;

    // Poziom zakończony (wygrana lub porażka) - wtedy nie wolno już pauzować gry.
    public bool LevelEnded { get; private set; }
    public void EndLevel() => LevelEnded = true;

    [Header("Layer Mask of Invisible Wall")]
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StepSoundManager stepSoundManager;

    [Header("Falling")]
    [Tooltip("Przyspieszenie kątowe przechylania się bloku przez krawędź (stopnie/s²).")]
    [SerializeField] private float fallTipAcceleration = 1500f;
    [Tooltip("O ile stopni blok przechyla się wokół krawędzi, zanim zacznie swobodnie spadać.")]
    [SerializeField] private float fallTipAngle = 90f;

    // Przechylanie przez krawędź przed swobodnym spadaniem (jak w Bloxorz).
    private bool isTipping;
    private Vector3 fallPivot, fallAxis, fallStartPosition;
    private Quaternion fallStartRotation;
    private float fallAngle, fallSpeed;

    [Header("Skin Changer")]
    private Renderer playerRenderer;

    // Skin w Awake, bo druga kostka w trybie dwóch kostek ma wyłączony skrypt i nie dostaje Start.
    void Awake()
    {
        playerRenderer = GetComponent<Renderer>();
        if (PlayerColorManager.Instance != null)
        {
            BlockSkin.Apply(playerRenderer, PlayerColorManager.Instance.SelectedMaterial);
        }
    }

    void Start()
    {
        isRotating = false;
        canMove = true;
        isFalling = false;
        scale = transform.localScale / 2f;
        rb = GetComponent<Rigidbody>();
        if (boxCollider == null) boxCollider = GetComponent<Collider>();
        FreezeAllAxes();
    }

    void Update()
    {
        if (isFalling)
        {
            if (isTipping) UpdateFallTip();
            return;
        }

        if (canMove)
        {
            if (isRotating)
            {
                UpdateRoll();
            }
            else if (GameInput.MoveUp.WasPressedThisFrame()) Rotate(DirectionForceApplier.W);
            else if (GameInput.MoveLeft.WasPressedThisFrame()) Rotate(DirectionForceApplier.A);
            else if (GameInput.MoveDown.WasPressedThisFrame()) Rotate(DirectionForceApplier.S);
            else if (GameInput.MoveRight.WasPressedThisFrame()) Rotate(DirectionForceApplier.D);
        }
    }

    private void UpdateRoll()
    {
        totalRotation = Mathf.Min(totalRotation + rotationSpeed * Time.deltaTime, 90f);

        if (totalRotation >= 90f)
        {
            FinishRoll();
            return;
        }

        Quaternion step = Quaternion.AngleAxis(rollAngleSign * totalRotation, axis);
        transform.SetPositionAndRotation(pivot + step * (rollStartPosition - pivot), step * rollStartRotation);
    }

    private void FinishRoll()
    {
        transform.SetPositionAndRotation(rollTargetPosition, GetRestingRotation());
        totalRotation = 90f;
        isRotating = false;
        moveTracker.AddMove();

        if (!TryStartFallIfUnsupported())
        {
            DetectSurface();
        }
    }

    // Prostopadłościan jest symetryczny, więc jego wygląd zależy wyłącznie od tego,
    // wzdłuż której osi leży najdłuższy bok. Zamiast kumulować rotację (i błędy),
    // po każdym ruchu ustawiamy jedną, dokładną rotację dla danej orientacji.
    private Quaternion GetRestingRotation()
    {
        if (scale.x > scale.y && scale.x > scale.z) return Quaternion.Euler(0f, 0f, 90f);
        if (scale.z > scale.y && scale.z > scale.x) return Quaternion.Euler(90f, 0f, 0f);
        return Quaternion.identity;
    }

    void Rotate(DirectionForceApplier direction)
    {
        rotationDirection = direction;
        isRotating = true;
        totalRotation = 0f;
        rollStartPosition = transform.position;
        rollStartRotation = transform.rotation;

        // Przesunięcie środka bloku po przetoczeniu o 90° wokół dolnej krawędzi
        // wynika wprost z wymiarów (wartości 0.5 / 1 są dokładne w float).
        Vector3 offset = Vector3.zero;
        switch (rotationDirection)
        {
            case DirectionForceApplier.D:
                pivot = rollStartPosition + new Vector3(scale.x, -scale.y, 0);
                offset = new Vector3(scale.x + scale.y, scale.x - scale.y, 0);
                break;
            case DirectionForceApplier.A:
                pivot = rollStartPosition + new Vector3(-scale.x, -scale.y, 0);
                offset = new Vector3(-(scale.x + scale.y), scale.x - scale.y, 0);
                break;
            case DirectionForceApplier.W:
                pivot = rollStartPosition + new Vector3(0, -scale.y, scale.z);
                offset = new Vector3(0, scale.z - scale.y, scale.z + scale.y);
                break;
            case DirectionForceApplier.S:
                pivot = rollStartPosition + new Vector3(0, -scale.y, -scale.z);
                offset = new Vector3(0, scale.z - scale.y, -(scale.z + scale.y));
                break;
        }
        rollTargetPosition = rollStartPosition + offset;
        rollAngleSign = (rotationDirection == DirectionForceApplier.A) || (rotationDirection == DirectionForceApplier.W) ? 1f : -1f;

        if ((rotationDirection == DirectionForceApplier.D) || (rotationDirection == DirectionForceApplier.A))
        {
            axis = Vector3.forward;
            (scale.y, scale.x) = (scale.x, scale.y);
        }
        else
        {
            axis = Vector3.right;
            (scale.y, scale.z) = (scale.z, scale.y);
        }
    }

    void DetectSurface()
    {
        Ray ray = new(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            string surfaceTag = hit.collider.tag;
            stepSoundManager.PlaySound(surfaceTag);
        }
    }

    public void FreezeAllAxes()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    #region Falling
    // Sprawdza, czy po ruchu każda część bloku stoi na płycie. Jeśli nie - zaczyna spadanie:
    // - gdy jedna połowa leżącego bloku wisi w powietrzu, blok przechyla się przez krawędź płyty,
    // - gdy blok nie ma żadnego podparcia, toczy się dalej wokół krawędzi, przez którą właśnie przeszedł.
    private bool TryStartFallIfUnsupported() => TryStartFallIfUnsupported(true);

    // Sprawdzenie podparcia bez ruchu bloku - np. gdy most zniknie pod stojącym graczem.
    // Bez podparcia blok spada prosto w dół (nie ma ruchu, który mógłby kontynuować).
    public void CheckSupportNow()
    {
        if (isFalling || isRotating || !enabled || !gameObject.activeInHierarchy) return;
        TryStartFallIfUnsupported(false);
    }

    // Czy blok stoi pionowo (nieruchomo) na polu o podanym środku.
    public bool IsStandingUprightAt(Vector3 cellCenter)
    {
        return !isFalling && !isRotating && enabled && gameObject.activeInHierarchy
               && scale.y > scale.x && scale.y > scale.z
               && Mathf.Abs(transform.position.x - cellCenter.x) < 0.1f
               && Mathf.Abs(transform.position.z - cellCenter.z) < 0.1f;
    }

    private bool TryStartFallIfUnsupported(bool afterRoll)
    {
        float bottomY = transform.position.y - scale.y;
        GetFootprint(out Vector3 cellA, out Vector3 cellB);
        bool supportedA = IsCellSupported(cellA, bottomY);
        bool supportedB = IsCellSupported(cellB, bottomY);

        if (supportedA && supportedB) return false;

        if (supportedA != supportedB)
        {
            Vector3 hanging = supportedA ? cellB : cellA;
            Vector3 resting = supportedA ? cellA : cellB;
            Vector3 outward = hanging - resting;
            outward.y = 0f;
            outward.Normalize();

            Vector3 edge = (hanging + resting) / 2f;
            edge.y = bottomY;
            StartFall(edge, Vector3.Cross(Vector3.up, outward), 0f);
        }
        else if (afterRoll)
        {
            StartFall(pivot, axis * rollAngleSign, rotationSpeed);
        }
        else
        {
            PreparPlayerToFallDown();
        }
        return true;
    }

    // Środki pól, które zajmuje blok (dla jednego pola oba punkty są takie same).
    private void GetFootprint(out Vector3 cellA, out Vector3 cellB)
    {
        Vector3 position = transform.position;
        Vector3 offset = Vector3.zero;
        if (scale.x > scale.y && scale.x > scale.z) offset = new Vector3(scale.x - 0.5f, 0f, 0f);
        else if (scale.z > scale.y && scale.z > scale.x) offset = new Vector3(0f, 0f, scale.z - 0.5f);

        cellA = position - offset;
        cellB = position + offset;
    }

    // Cienka "sonda" tuż pod spodem bloku. Liczą się tylko stałe collidery (płyty, mosty, meta),
    // więc triggery przycisków i stare pasy spadania nie są traktowane jak podłoże.
    private bool IsCellSupported(Vector3 cell, float bottomY)
    {
        Vector3 center = new(cell.x, bottomY - 0.05f, cell.z);
        Vector3 halfExtents = new(0.3f, 0.04f, 0.3f);
        int mask = ~(1 << gameObject.layer);
        return IsMetaCell(cell, bottomY)
            || IsGroundTileAt(cell, bottomY)
            || HasSolidColliderInOwnScene(center, halfExtents, mask);
    }

    private bool HasSolidColliderInOwnScene(Vector3 center, Vector3 halfExtents, int mask)
    {
        foreach (Collider hit in Physics.OverlapBox(center, halfExtents, Quaternion.identity, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.gameObject.scene == gameObject.scene) return true;
        }
        return false;
    }

    // Płyty na warstwie "Ground" (zwykłe płyty i wszystkie przyciski na ziemi). Przyciski nie mają
    // stałego collidera - tylko triggery, często wyłączone do momentu aktywacji - dlatego
    // podłoże sprawdzamy po pozycji i rozmiarze samej płyty, a nie przez fizykę.
    private List<Transform> groundTiles;

    private bool IsGroundTileAt(Vector3 cell, float bottomY)
    {
        groundTiles ??= FindGroundTiles();

        foreach (Transform tile in groundTiles)
        {
            if (tile == null || !tile.gameObject.activeInHierarchy) continue;

            Vector3 position = tile.position;
            Vector3 size = tile.lossyScale;
            float top = position.y + Mathf.Abs(size.y) / 2f;
            float halfWidth = Mathf.Max(0.1f, Mathf.Max(Mathf.Abs(size.x), Mathf.Abs(size.z)) / 2f - 0.05f);

            if (Mathf.Abs(top - bottomY) < 0.15f
                && Mathf.Abs(position.x - cell.x) < halfWidth
                && Mathf.Abs(position.z - cell.z) < halfWidth)
            {
                return true;
            }
        }
        return false;
    }

    private List<Transform> FindGroundTiles()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        List<Transform> tiles = new();

        // Tylko obiekty ze sceny gracza - pomija poziom, który właśnie się wyładowuje.
        // Korzenie płyt (Ground, przyciski) mają środek na y = -1.1 i skalę (1, 0.2, 1),
        // więc ich pozycja i skala wyznaczają powierzchnię płyty.
        foreach (Transform candidate in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (candidate.gameObject.layer == groundLayer && candidate.gameObject.scene == gameObject.scene)
            {
                tiles.Add(candidate);
            }
        }
        return tiles;
    }

    // Dziura mety nie ma pod sobą płyty, ale jak w Bloxorz działa jak podłoże:
    // stojąc na niej pionowo wygrywasz (obsługuje to Meta), a leżąc w poprzek nie spadasz.
    private bool IsMetaCell(Vector3 cell, float bottomY)
    {
        foreach (Meta meta in Meta.ActiveMetas)
        {
            if (meta.gameObject.scene != gameObject.scene) continue;

            // HolePosition to środek bloku stojącego pionowo w mecie (spód + 1).
            Vector3 hole = meta.HolePosition;
            if (Mathf.Abs(hole.x - cell.x) < 0.1f && Mathf.Abs(hole.z - cell.z) < 0.1f
                && Mathf.Abs(hole.y - (bottomY + 1f)) < 0.5f)
            {
                return true;
            }
        }
        return false;
    }

    private void StartFall(Vector3 tipPivot, Vector3 tipAxis, float startSpeed)
    {
        isFalling = true;
        EndLevel();
        isTipping = true;
        canMove = false;
        isRotating = false;

        fallPivot = tipPivot;
        fallAxis = tipAxis.normalized;
        fallStartPosition = transform.position;
        fallStartRotation = transform.rotation;
        fallAngle = 0f;
        fallSpeed = startSpeed;

        SoundManager.Instance.PlaySound(SoundClip.LoseSound);
    }

    private void UpdateFallTip()
    {
        fallSpeed += fallTipAcceleration * Time.deltaTime;
        fallAngle = Mathf.Min(fallAngle + fallSpeed * Time.deltaTime, fallTipAngle);

        Quaternion step = Quaternion.AngleAxis(fallAngle, fallAxis);
        transform.SetPositionAndRotation(fallPivot + step * (fallStartPosition - fallPivot), step * fallStartRotation);

        if (fallAngle >= fallTipAngle)
        {
            ReleaseToPhysics();
        }
    }

    // Po przechyleniu blok spada swobodnie, zachowując prędkość z animacji.
    // Kolizje z płytami są wyłączone, żeby nie zahaczał o krawędzie - liczy się tylko ResetCollider.
    private void ReleaseToPhysics()
    {
        isTipping = false;
        IgnoreCollisionsExceptReset();

        Vector3 angularVelocity = fallAxis * (fallSpeed * Mathf.Deg2Rad);
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        rb.maxAngularVelocity = Mathf.Max(rb.maxAngularVelocity, angularVelocity.magnitude);
        rb.linearVelocity = Vector3.Cross(angularVelocity, transform.position - fallPivot);
        rb.angularVelocity = angularVelocity;
    }

    private void IgnoreCollisionsExceptReset()
    {
        if (boxCollider == null) return;

        foreach (Collider other in Physics.OverlapSphere(transform.position, 200f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (other != boxCollider && !other.CompareTag("ResetCollider"))
            {
                Physics.IgnoreCollision(boxCollider, other);
            }
        }
    }

    // Awaryjne spadanie wywoływane z zewnątrz (np. FallingForceApplier, gdy most zniknie pod graczem).
    public void PreparPlayerToFallDown()
    {
        if (isFalling) return;

        isFalling = true;
        EndLevel();
        canMove = false;
        isRotating = false;
        fallPivot = transform.position;
        fallAxis = Vector3.right;
        fallSpeed = 0f;
        SoundManager.Instance.PlaySound(SoundClip.LoseSound);
        ReleaseToPhysics();
    }
    #endregion

}

public enum DirectionForceApplier
{
    None,
    W,
    S,
    A,
    D
}
