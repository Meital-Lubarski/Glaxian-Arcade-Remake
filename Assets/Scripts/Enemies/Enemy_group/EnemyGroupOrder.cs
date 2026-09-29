using UnityEngine;

public class EnemyGroupOrder : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [Header("46 total (change counts per row as needed)")]
    [SerializeField] private int[] enemiesPerRow = { 2, 6, 8, 10, 10, 10 }; // =46

    [Header("Layout")]
    [SerializeField] private float spacingX = 1.2f;
    [SerializeField] private float spacingY = 0.4f;
    [SerializeField] private Vector2 topCenterLocal = new Vector2(0f, 3.5f);
    [Header("Special spacing")]
    [SerializeField] private int topRowEmptySlotsBetween = 2;
    
    [Header("Animator controller per row (top -> bottom)")]
    [SerializeField] private RuntimeAnimatorController[] controllerPerRow;

    [SerializeField] private EnemyData[] dataPerRow;
    
    private const int TopRowIndex = 0;
    private const int TopRowEnemyCount = 2;
    
    private void ApplyRoleByRow(GameObject go, int r)
    {
        if (go == null)
            return;

        EnemyRoleTag tag = go.GetComponent<EnemyRoleTag>();
        if (tag == null)
            tag = go.AddComponent<EnemyRoleTag>();

        if (r == 0)
            tag.role = EnemyRole.Flagship;
        else if (r == 1)
            tag.role = EnemyRole.Red;
        else
            tag.role = EnemyRole.Normal;
        EnemyIdentity identity = go.GetComponent<EnemyIdentity>();
        if (identity == null)
        {
            identity = go.AddComponent<EnemyIdentity>();
        }

        if (dataPerRow != null && r < dataPerRow.Length)
        {
            identity.enemyData =  dataPerRow[r];
        }
    }

    
    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not set!");
            return;
        }

        ClearChildren();

        for (int r = 0; r < enemiesPerRow.Length; r++)
        {
            int count = enemiesPerRow[r];
            float y = topCenterLocal.y - r * spacingY;

            if (BuildTopRow(r, count, y))
                continue;

            BuildRegularRow(r, count, y);
        }
    }

    private bool BuildTopRow(int r, int count, float y)
    {
        if (!(r == TopRowIndex && count == TopRowEnemyCount))
        {
            return false;
        }
        float distance = (topRowEmptySlotsBetween + 1) * spacingX;
        for (int c = 0; c < TopRowEnemyCount; c++)
        {
            float xOffset = (c == 0) ? -distance * 0.5f : distance * 0.5f;
            GameObject go = Instantiate(enemyPrefab, transform);
            ApplyRoleByRow(go, r);
            go.transform.localPosition = new Vector3(topCenterLocal.x + xOffset, y, 0f);
            EnemySlot s = go.GetComponent<EnemySlot>();
            s.Setup(transform, go.transform.localPosition);
            go.name = $"Enemy_r{r}_c{c}";
            AttachAnimatorController(go, r);
        }
        return true;
    }
    
    private void BuildRegularRow(int r, int count, float y)
    {
        float rowWidth = (count - 1) * spacingX;
        float startX = topCenterLocal.x - rowWidth * 0.5f;

        for (int c = 0; c < count; c++)
        {
            GameObject go = Instantiate(enemyPrefab, transform);
            ApplyRoleByRow(go, r);

            go.transform.localPosition = new Vector3(
                startX + c * spacingX,
                y,
                0f
            );

            EnemySlot s = go.GetComponent<EnemySlot>();
            s.Setup(transform, go.transform.localPosition);

            go.name = $"Enemy_r{r}_c{c}";

            AttachAnimatorController(go, r);
        }
    }
    
    private void AttachAnimatorController(GameObject go, int r)
    {
        Animator anim = go.GetComponent<Animator>();
        if (anim == null)
            return;

        if (controllerPerRow == null || r >= controllerPerRow.Length)
            return;

        RuntimeAnimatorController controller = controllerPerRow[r];
        if (controller == null)
            return;

        anim.runtimeAnimatorController = controller;
    }

    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(child);
            else Destroy(child);
#else
            Destroy(child);
#endif
        }
    }
}