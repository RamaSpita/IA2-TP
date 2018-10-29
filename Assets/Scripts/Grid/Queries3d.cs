using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queries3d : MonoBehaviour
{
    public bool isBox;
    public float radius = 20f;
    public SpatialGrid3d targetGrid;
    public float width = 15f;
    public float height = 30f;
    public float deep = 30f;
    public IEnumerable<GridEntity> selected = new List<GridEntity>();

    public IEnumerable<GridEntity> Query()
    {
        if (isBox)
        {
            var h = height * 0.5f;
            var w = width * 0.5f;
            var d = deep * 0.5f;
            //posicion inicial --> esquina superior izquierda de la "caja"
            //posición final --> esquina inferior derecha de la "caja"
            //como funcion para filtrar le damos una que siempre devuelve true, para que no filtre nada.
            return targetGrid.Query(
                transform.position + new Vector3(-w, -d, -h),
                transform.position + new Vector3(w, d, h),
                x => true);
        }
        else
        {
            //creo una "caja" con las dimensiones deseadas, y luego filtro segun distancia para formar el círculo
            return targetGrid.Query(
                transform.position + new Vector3(-radius, -radius, -radius),
                transform.position + new Vector3(radius, radius, radius),
                x => {
                    var position = x - transform.position;
                    return position.sqrMagnitude < radius * radius;
                });
        }
    }

    void OnDrawGizmos()
    {
        if (targetGrid == null)
            return;

        //Flatten the sphere we're going to draw
        Gizmos.color = Color.cyan;
        if (isBox)
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, deep));
        else
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        if (Application.isPlaying)
        {
            selected = Query();
        }
    }


    private void OnGUI()
    {
        GUI.Label( new Rect(0,0,20,20), "HOLA");
    }
}
