using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectObject : MonoBehaviour
{
    public Camera playerCamera;
    public FirstPersonController firstPersonController; // Referência ao FirstPersonController
    public float mouseSensitivity = 1.5f;
    public float inspectDistance = 1f; // Distância fixa para o objeto ficar à frente da câmera (1 metro)

    private bool isInspecting = false;
    private GameObject inspectedObject;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody objectRigidbody;

    void Update()
    {
        if (firstPersonController.cameraCanMove && Input.GetKeyDown(KeyCode.E))
        {
            if (!isInspecting)
            {
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, inspectDistance * 3))
                {
                    if (hit.collider.CompareTag("Mexivel"))
                    {
                        inspectedObject = hit.collider.gameObject;
                        originalPosition = inspectedObject.transform.position;
                        originalRotation = inspectedObject.transform.rotation;

                        // Salva e desativa o Rigidbody, se houver
                        objectRigidbody = inspectedObject.GetComponent<Rigidbody>();
                        if (objectRigidbody != null)
                        {
                            objectRigidbody.isKinematic = true;
                        }

                        // Coloca o objeto diretamente à frente da câmera a uma distância fixa
                        inspectedObject.transform.position = playerCamera.transform.position + (playerCamera.transform.forward / 11) * inspectDistance;
                        inspectedObject.transform.rotation = playerCamera.transform.rotation;
                        inspectedObject.transform.SetParent(playerCamera.transform);

                        isInspecting = true;
                        firstPersonController.playerCanMove = false;
                        firstPersonController.cameraCanMove = false;
                    }
                }
            }
        }

        // Sai do modo de inspeção ao pressionar ESC
        if (isInspecting && Input.GetKeyDown(KeyCode.Q))
        {
            ExitInspectionMode();
        }

        if (isInspecting && inspectedObject != null)
        {
            // Rotaciona o objeto com o movimento do mouse
            float rotationX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float rotationY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            inspectedObject.transform.Rotate(playerCamera.transform.up, -rotationX, Space.World);
            inspectedObject.transform.Rotate(playerCamera.transform.right, rotationY, Space.World);
        }
    }

    private void ExitInspectionMode()
    {
        isInspecting = false;

        // Restaura a posição e a rotação originais
        inspectedObject.transform.SetParent(null);
        inspectedObject.transform.position = originalPosition;
        inspectedObject.transform.rotation = originalRotation;

        // Reativa o Rigidbody, se havia um
        if (objectRigidbody != null)
        {
            objectRigidbody = null;
        }

        inspectedObject = null;
        firstPersonController.playerCanMove = true;
        firstPersonController.cameraCanMove = true;
    }
}