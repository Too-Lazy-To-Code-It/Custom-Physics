using UnityEngine;

public class QuadruplePendulum : MonoBehaviour
{
    // Param�tres physiques
    public float l1 = 1.0f, l2 = 1.0f, l3 = 1.0f, l4 = 1.0f; // Longueurs des pendules
    public float m1 = 1.0f, m2 = 1.0f, m3 = 1.0f, m4 = 1.0f; // Masses des pendules
    public float g = 9.81f; // Gravit�

    // Amortissement, sous-�tapes, et it�rations
    public float damping = 0.99f;
    public int substeps = 40;
    public int solverIterations = 100;

    // Angles initiaux et leurs vitesses
    private float theta1, theta2, theta3, theta4;
    private float omega1 = 2.0f, omega2 = 2.0f, omega3 = 2.0f, omega4 = 2.0f;

    // R�f�rences aux objets visuels pour les masses
    public Transform mass1, mass2, mass3, mass4;

    // R�f�rences aux LineRenderers pour les tiges
    public LineRenderer line1, line2, line3, line4;

    void Start()
    {
        // Initialiser les angles initiaux
        theta1 = Mathf.PI / 4;
        theta2 = Mathf.PI / 3;
        theta3 = Mathf.PI / 6;
        theta4 = Mathf.PI / 8;

        SetupLine(line1);
        SetupLine(line2);
        SetupLine(line3);
        SetupLine(line4);
    }

    void Update()
    {
        float dt = Time.deltaTime / substeps;

        for (int i = 0; i < substeps; i++)
        {
            for (int j = 0; j < solverIterations; j++)
            {
                SimulatePendulum(dt);
            }
            ApplyDamping();
        }

        UpdatePositions();
    }

    void SimulatePendulum(float dt)
    {
        // Calculs simplifi�s d'acc�l�rations
        float alpha1 = -g * Mathf.Sin(theta1);
        float alpha2 = -g * Mathf.Sin(theta2);
        float alpha3 = -g * Mathf.Sin(theta3);
        float alpha4 = -g * Mathf.Sin(theta4);

        // Mise � jour des vitesses angulaires
        omega1 += alpha1 * dt;
        omega2 += alpha2 * dt;
        omega3 += alpha3 * dt;
        omega4 += alpha4 * dt;

        // Mise � jour des angles
        theta1 += omega1 * dt;
        theta2 += omega2 * dt;
        theta3 += omega3 * dt;
        theta4 += omega4 * dt;
    }

    void ApplyDamping()
    {
        omega1 *= damping;
        omega2 *= damping;
        omega3 *= damping;
        omega4 *= damping;
    }

    void UpdatePositions()
    {
        // Position du premier pendule
        Vector3 pos1 = new Vector3(l1 * Mathf.Sin(theta1), -l1 * Mathf.Cos(theta1), 0);
        mass1.position = pos1;

        // Position du deuxi�me pendule
        Vector3 pos2 = pos1 + new Vector3(l2 * Mathf.Sin(theta2), -l2 * Mathf.Cos(theta2), 0);
        mass2.position = pos2;

        // Position du troisi�me pendule
        Vector3 pos3 = pos2 + new Vector3(l3 * Mathf.Sin(theta3), -l3 * Mathf.Cos(theta3), 0);
        mass3.position = pos3;

        // Position du quatri�me pendule
        Vector3 pos4 = pos3 + new Vector3(l4 * Mathf.Sin(theta4), -l4 * Mathf.Cos(theta4), 0);
        mass4.position = pos4;

        // Mise � jour des lignes
        line1.SetPosition(0, Vector3.zero);
        line1.SetPosition(1, mass1.position);

        line2.SetPosition(0, mass1.position);
        line2.SetPosition(1, mass2.position);

        line3.SetPosition(0, mass2.position);
        line3.SetPosition(1, mass3.position);

        line4.SetPosition(0, mass3.position);
        line4.SetPosition(1, mass4.position);
    }

    void SetupLine(LineRenderer line)
    {
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;
        line.endColor = Color.green;
    }
}
