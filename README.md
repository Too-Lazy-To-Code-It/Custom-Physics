Global files:

    QuaretnionUtility: 
      
      Quaternion CreateQuaternion(Vector3 axis, float angle)
      
      Matrix4x4 CreateRotationMatrixFromQuaternion(Quaternion q)
      
      Vector3 MultiplyPoint3x4(Matrix4x4 matrix, Vector3 point)
    
    MatrixUtility:
      
      float[,] MultiplyMatrices(float[,] mat1, float[,] mat2)
      
      float[,] inverseA(float[,] A)
    
    RK4Utility:
      
      (Vector3, Vector3) RK4SolverMethod(Vector3 pos, Vector3 vel, Vector3 acc, float dt)
    
    VectorUtility:
      
      Vector3 vectorBA(Vector3 A, Vector3 B)
      
      float normBA(Vector3 vecBA)
