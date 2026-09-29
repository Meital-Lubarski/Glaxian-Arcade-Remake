using UnityEngine;

public class EnemySlot : MonoBehaviour
{
    public Transform FormationRoot { get; private set; }
    public Vector3 HomeLocalPosition { get;  private set; }

    public void Setup(Transform formationRoot, Vector3 homeLocalPosition)
    {
        FormationRoot = formationRoot;
        HomeLocalPosition = homeLocalPosition;
    }


    public Vector3 GetHomeWorldPosition()
    {
        if (FormationRoot != null)
        {
            return FormationRoot.TransformPoint(HomeLocalPosition);
        }
        return transform.position;
    }
}
