using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TRSConverter 
{

    public static float ConvertDegToRad(float degrees)
    {
        return ((float)Mathf.PI / (float)180) * degrees;
    }

    public static Matrix4x4 GetTranslationMatrix(Vector3 position)
    {
        return new Matrix4x4(new Vector4(1, 0, 0, 0),
                             new Vector4(0, 1, 0, 0),
                             new Vector4(0, 0, 1, 0),
                             new Vector4(position.x, position.y, position.z, 1));
    }

    public static Matrix4x4 GetRotationMatrix(Vector3 anglesDeg)
    {
        anglesDeg = new Vector3(ConvertDegToRad(anglesDeg[0]), ConvertDegToRad(anglesDeg[1]), ConvertDegToRad(anglesDeg[2]));

        Matrix4x4 rotationX = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                            new Vector4(0, Mathf.Cos(anglesDeg[0]), Mathf.Sin(anglesDeg[0]), 0),
                                            new Vector4(0, -Mathf.Sin(anglesDeg[0]), Mathf.Cos(anglesDeg[0]), 0),
                                            new Vector4(0, 0, 0, 1));

        Matrix4x4 rotationY = new Matrix4x4(new Vector4(Mathf.Cos(anglesDeg[1]), 0, -Mathf.Sin(anglesDeg[1]), 0),
                                            new Vector4(0, 1, 0, 0),
                                            new Vector4(Mathf.Sin(anglesDeg[1]), 0, Mathf.Cos(anglesDeg[1]), 0),
                                            new Vector4(0, 0, 0, 1));

        Matrix4x4 rotationZ = new Matrix4x4(new Vector4(Mathf.Cos(anglesDeg[2]), Mathf.Sin(anglesDeg[2]), 0, 0),
                                            new Vector4(-Mathf.Sin(anglesDeg[2]), Mathf.Cos(anglesDeg[2]), 0, 0),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(0, 0, 0, 1));

        return rotationX * rotationY * rotationZ;
    }

    public static Matrix4x4 GetScaleMatrix(Vector3 scale)
    {
        return new Matrix4x4(new Vector4(scale.x, 0, 0, 0),
                             new Vector4(0, scale.y, 0, 0),
                             new Vector4(0, 0, scale.z, 0),
                             new Vector4(0, 0, 0, 1));
    }

    public static Matrix4x4 Get_TRS_Matrix(Vector3 position, Vector3 rotationAngles, Vector3 scale)
    {
        return GetTranslationMatrix(position) * GetRotationMatrix(rotationAngles) * GetScaleMatrix(scale);
    }
}