using UnityEngine;
public class TriplePendulum : MonoBehaviour
{
    // Param�tres physiques
    public float l1 = 1.0f, l2 = 1.0f, l3 = 1.0f; // Longueurs des pendules
    public float m1 = 1.0f, m2 = 1.0f, m3 = 1.0f; // Masses des pendules
    public float g = 9.81f; // Gravit�

    // Amortissement, sous-�tapes, et it�rations
    public float damping = 0.99995f;
    public int substeps = 80;
    public int solverIterations = 1;

    // Angles initiaux et leurs vitesses
    private float theta1, theta2, theta3;
    private float omega1 = 2.0f, omega2 = 2.0f, omega3 = 2.0f;  // Plus rapide

    // R�f�rences aux objets visuels pour les masses
    public Transform mass1, mass2, mass3;

    // R�f�rences aux LineRenderers pour les tiges
    public LineRenderer line1, line2, line3;

    void Start()
    {
        // Initialiser les angles initiaux
        theta1 = Mathf.PI / 4;
        theta2 = Mathf.PI / 3;
        theta3 = Mathf.PI / 6;



        SetupLine(line1);
        SetupLine(line2);
        SetupLine(line3);
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
        float alpha1 = -g * (2 * m1 + m2) * Mathf.Sin(theta1)
                           - m2 * g * Mathf.Sin(theta1 - 2 * theta2)
                           - 2 * Mathf.Sin(theta1 - theta2) * m2
                           * (omega2 * omega2 * l2 + omega1 * omega1 * l1 * Mathf.Cos(theta1 - theta2));

        float alpha2 = 2 * Mathf.Sin(theta1 - theta2)
                       * (omega1 * omega1 * l1 * (m1 + m2) + g * (m1 + m2) * Mathf.Cos(theta1)
                       + omega2 * omega2 * l2 * m2 * Mathf.Cos(theta1 - theta2));

        // Ajuster l'alpha pour le 3�me pendule (ajoutez le calcul de l'acc�l�ration de la masse 3 si n�cessaire)
        float alpha3 = -g * Mathf.Sin(theta3);   // Calcul de l'acc�l�ration pour le troisi�me pendule

        omega1 += alpha1 * dt;
        omega2 += alpha2 * dt;
        omega3 += alpha3 * dt;

        //// Ajustement pour que les angles des pendules convergent vers un alignement
        //if (Mathf.Abs(theta1 - theta2) < 0.01f && Mathf.Abs(theta2 - theta3) < 0.01f)
        //{
        //    // Ajuster les angles pour les rendre align�s
        //    theta1 = theta2 = theta3;
        //}

        // Mettre � jour les angles
        theta1 += omega1 * dt;
        theta2 += omega2 * dt;
        theta3 += omega3 * dt;
    }

    void ApplyDamping()
    {
        omega1 *= damping;
        omega2 *= damping;
        omega3 *= damping;
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

        // Mettre � jour les lignes pour chaque tige
        // Point de d�part du premier pendule (base)
        line1.SetPosition(0, Vector3.zero);
        line1.SetPosition(1, mass1.position); // Extr�mit� du premier pendule

        // Point de d�part du deuxi�me pendule
        line2.SetPosition(0, mass1.position);
        line2.SetPosition(1, mass2.position); // Extr�mit� du deuxi�me pendule

        // Point de d�part du troisi�me pendule
        line3.SetPosition(0, mass2.position);
        line3.SetPosition(1, mass3.position); // Extr�mit� du troisi�me pendule
    }

    void SetupLine(LineRenderer line)
    {
        // Configurer les propri�t�s des lignes
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.red;  // Couleur de d�part
        line.endColor = Color.green;  // Couleur de fin
    }
}