using UnityEngine;
using UnityEngine.Splines;

public class DiveSplineLibrary : MonoBehaviour
{
    public static DiveSplineLibrary Instance { get; private set; }

    [Header("Dive Splines")]
    [SerializeField] private SplineContainer[] left;
    [SerializeField] private SplineContainer[] right;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public SplineContainer Pick(float homeLocalX)
    {
        return (homeLocalX < 0f) ? PickRandomNonNull(left) : PickRandomNonNull(right);
    }

    private static SplineContainer PickRandomNonNull(SplineContainer[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            return null;
        }
        int start = Random.Range(0, arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            int idx = (start + i) % arr.Length;
            if (arr[idx] != null)
            {
                return arr[idx];
            }
        }
        return null;
    }
}