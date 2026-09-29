using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void SetLoaded(bool loaded)
    {
        if (_animator != null)
        {
            _animator.SetBool("IsLoaded", loaded);
        }
        
    }
    public void TriggerExplosion()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Explode");
        }
    }
}
