using UnityEngine;
using UnityEngine.Animations;

public class SetupDrawerKeyConstraints : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform childTransform;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0f);

    private PositionConstraint posConstraint;
    private RotationConstraint rotConstraint;

    void Start()
    {
        SetupPositionConstraint();
        SetupRotationConstraint();

        enabled = false; 
    }

    void SetupPositionConstraint()
    {
        posConstraint = childTransform.gameObject.AddComponent<PositionConstraint>();

        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parentTransform,
            weight = 1.0f
        };

        posConstraint.AddSource(source);
        posConstraint.translationOffset = localOffset;
        posConstraint.constraintActive = true;
        posConstraint.locked = true;
    }

    void SetupRotationConstraint()
    {
        rotConstraint = childTransform.gameObject.AddComponent<RotationConstraint>();

        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = parentTransform,
            weight = 1.0f
        };

        rotConstraint.AddSource(source);
        rotConstraint.constraintActive = true;
        rotConstraint.locked = true;
    }

    public void EnableKeyInteraction(bool enable)
    {
        posConstraint.constraintActive = !enable;
        rotConstraint.constraintActive = !enable;

        if (enable)
        {
            var rb = childTransform.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }
}