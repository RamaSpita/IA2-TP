using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SpatialGrid3d : MonoBehaviour
{
    #region Variables
    //punto de inicio de la grilla en X
    public float x;
    //punto de inicio de la grilla en Z
    public float z;
    //punto de inicio de la grilla en Z
    public float y;
    //ancho de las celdas
    public float cellWidth;
    //alto de las celdas
    public float cellHeight;
    //alto de las celdas
    public float cellDeep;
    //cantidad de columnas (el "ancho" de la grilla)
    public int width;
    //cantidad de filas (el "alto" de la grilla)
    public int height;
    //cantidad de filas (el "alto" de la grilla)
    public int deep;

    //ultimas posiciones conocidas de los elementos, guardadas para comparación.
    private Dictionary<GridEntity, Tuple<int, int, int>> lastPositions;
    //los "contenedores"
    private HashSet<GridEntity>[,,] buckets;

    //el valor de posicion que tienen los elementos cuando no estan en la zona de la grilla.
    /*
     Const es implicitamente statica
     const tengo que ponerle el valor apenas la declaro, readonly puedo hacerlo en el constructor.
     Const solo sirve para tipos de dato primitivos.
     */
    readonly public Tuple<int, int, int> Outside = Tuple.Create(-1, -1, -1);

    //Una colección vacía a devolver en las queries si no hay nada que devolver
    readonly public GridEntity[] Empty = new GridEntity[0];
    #endregion

    #region FUNCIONES
    private void Awake()
    {
        lastPositions = new Dictionary<GridEntity, Tuple<int, int, int>>();
        buckets = new HashSet<GridEntity>[width, height, deep];

        //creamos todos los hashsets
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                for (int k = 0; k < deep; k++)
                    buckets[i, j, k] = new HashSet<GridEntity>();

        //P/alumnos: por que no puedo usar OfType<>() despues del RecursiveWalker() aca?
        var ents = RecursiveWalker(transform)
            .Select(x => x.GetComponent<GridEntity>())
            .Where(x => x != null);

        foreach (var e in ents)
        {
            e.OnMove += UpdateEntity;
            UpdateEntity(e);
        }
    }

    public void UpdateEntity(GridEntity entity)
    {
        var lastPos = lastPositions.ContainsKey(entity) ? lastPositions[entity] : Outside;
        var currentPos = GetPositionInGrid(entity.gameObject.transform.position);

        //Misma posición, no necesito hacer nada
        if (lastPos.Equals(currentPos))
            return;

        //Lo "sacamos" de la posición anterior
        if (IsInsideGrid(lastPos))
            buckets[lastPos.Item1, lastPos.Item2, lastPos.Item3].Remove(entity);

        //Lo "metemos" a la celda nueva, o lo sacamos si salio de la grilla
        if (IsInsideGrid(currentPos))
        {
            buckets[currentPos.Item1, currentPos.Item2, currentPos.Item3].Add(entity);
            lastPositions[entity] = currentPos;
        }
        else
            lastPositions.Remove(entity);
    }

    public IEnumerable<GridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition)
    {
        var from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), Mathf.Min(aabbFrom.y, aabbTo.y), Mathf.Min(aabbFrom.z, aabbTo.z));
        var to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), Mathf.Max(aabbFrom.y, aabbTo.y), Mathf.Max(aabbFrom.z, aabbTo.z));

        var fromCoord = GetPositionInGrid(from);
        var toCoord = GetPositionInGrid(to);

        //¡Ojo que clampea a 0,0 el Outside! TODO: Checkear cuando descartar el query si estan del mismo lado
        fromCoord = Tuple.Create(Utility.Clampi(fromCoord.Item1, 0, width), Utility.Clampi(fromCoord.Item2, 0, height), Utility.Clampi(fromCoord.Item3, 0, deep));
        toCoord = Tuple.Create(Utility.Clampi(toCoord.Item1, 0, width), Utility.Clampi(toCoord.Item2, 0, height), Utility.Clampi(toCoord.Item3, 0, deep));

        if (!IsInsideGrid(fromCoord) && !IsInsideGrid(toCoord))
            return Empty;

        //TODO p/Alumno: ¿Cómo haría esto con un Aggregate en vez de generar posiciones?
        //TODO p/Alumno: Cambiar por Where/Take

        // Creamos tuplas de cada celda
        var cols = Generate(fromCoord.Item1, x => x + 1)
            .TakeWhile(x => x < width && x <= toCoord.Item1);

        var rows = Generate(fromCoord.Item2, y => y + 1)
            .TakeWhile(y => y < height && y <= toCoord.Item2);

        var deepCols = Generate(fromCoord.Item3, z => z + 1)
           .TakeWhile(z => z < deep && y <= toCoord.Item3);

        var cells = cols.SelectMany(
            col => rows.SelectMany(
            row => deepCols.Select(
                deepCol => Tuple.Create(col, row, deepCol)
                )
            )
        );

        // Iteramos las que queden dentro del criterio
        return cells
            .SelectMany(cell => buckets[cell.Item1, cell.Item2, cell.Item3])
            .Where(e =>
                from.x <= e.transform.position.x && e.transform.position.x <= to.x &&
                from.y <= e.transform.position.y && e.transform.position.y <= to.y &&
                from.z <= e.transform.position.z && e.transform.position.z <= to.z
            ).Where(x => filterByPosition(x.transform.position));
    }

    public Tuple<int, int , int> GetPositionInGrid(Vector3 pos)
    {
        //quita la diferencia, divide segun las celdas y floorea
        return Tuple.Create(Mathf.FloorToInt((pos.x - x - transform.position.x) / cellWidth),
                            Mathf.FloorToInt((pos.y - y - transform.position.y) / cellHeight),
                            Mathf.FloorToInt((pos.z - z - transform.position.z) / cellDeep));
    }

    public bool IsInsideGrid(Tuple<int, int, int> position)
    {
        //si es menor a 0 o mayor a width o height, no esta dentro de la grilla
        return 0 <= position.Item1 && position.Item1 < width &&
            0 <= position.Item2 && position.Item2 < height &&
            0 <= position.Item3 && position.Item3 < deep;
    }

    void OnDestroy()
    {
        var ents = RecursiveWalker(transform).Select(x => x.GetComponent<GridEntity>()).Where(x => x != null);
        foreach (var e in ents)
            e.OnMove -= UpdateEntity;
    }

    #region GENERATORS
    private static IEnumerable<Transform> RecursiveWalker(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform grandchild in RecursiveWalker(child))
                yield return grandchild;
            yield return child;
        }
    }

    IEnumerable<T> Generate<T>(T seed, Func<T, T> mutate)
    {
        T accum = seed;
        while (true)
        {
            yield return accum;
            accum = mutate(accum);
        }
    }
    #endregion

    #endregion

    #region GRAPHIC REPRESENTATION
    public bool AreGizmosShutDown;
    public bool activatedGrid;
    public bool showLogs = true;
    private void OnDrawGizmos()
    {
        var saCols = Generate(x, curr => curr + cellWidth).Take(width + 1);
        var saRows = Generate(y, curr => curr + cellHeight).Take(height + 1);
        var saDeepCols = Generate(x, curr => curr + cellDeep).Take(deep + 1);

        var colsAndDeepCols = saCols.Select(x => saDeepCols.Select(z=> Tuple.Create(x,z)));

        var colsAndDeepCols2 = colsAndDeepCols
                  .Select(colAndDeepCols => colAndDeepCols
                  .Select(colAndDeepCol => Tuple.Create(new Vector3(colAndDeepCol.Item1, y, colAndDeepCol.Item2),
                                           new Vector3(colAndDeepCol.Item1, y + cellHeight * height, colAndDeepCol.Item2))));

        var colsAndRows = saCols.Select(x => saRows.Select(z => Tuple.Create(x, z)));

        var colsAndRows2 = colsAndRows
                  .Select(colAndRows => colAndRows
                  .Select(colAndRow => Tuple.Create(new Vector3(colAndRow.Item1, colAndRow.Item2, z),
                                           new Vector3(colAndRow.Item1, colAndRow.Item2, z + cellDeep * deep))));

        var rowsAndDeepCols = saRows.Select(x => saDeepCols.Select(z => Tuple.Create(x, z)));

        var rowsAndDeepCols2 = rowsAndDeepCols
                  .Select(rowAndDeepCols => rowAndDeepCols
                  .Select(rowAndDeepCol => Tuple.Create(new Vector3(x, rowAndDeepCol.Item1, rowAndDeepCol.Item2),
                                           new Vector3(x + cellWidth * width, rowAndDeepCol.Item1, rowAndDeepCol.Item2))));


        var allLines = colsAndDeepCols2.Concat(colsAndRows2.Concat(rowsAndDeepCols2));

        foreach (var elem in allLines)
            foreach (var elem2 in elem)
                Gizmos.DrawLine(transform.position + elem2.Item1, transform.position + elem2.Item2);


        if (buckets == null || AreGizmosShutDown) return;

        var originalCol = GUI.color;
        GUI.color = Color.red;
        if (!activatedGrid)
        {
            IEnumerable<GridEntity> allElems = Enumerable.Empty<GridEntity>();
            foreach(var elem in buckets)
                allElems = allElems.Concat(elem);

            int connections = 0;
            foreach (var ent in allElems)
            {
                foreach(var neighbour in allElems.Where(x => x != ent))
                {
                    Gizmos.DrawLine(ent.transform.position, neighbour.transform.position);
                    connections++;
                }
                if(showLogs)
                    Debug.Log("tengo " + connections + " conexiones por individuo");
                connections = 0;
            }
        }
        else
        {
            int connections = 0;
            foreach (var elem in buckets)
            {
                foreach(var ent in elem)
                {
                    foreach (var n in elem.Where(x => x != ent))
                    {
                        Gizmos.DrawLine(ent.transform.position, n.transform.position);
                        connections++;
                    }
                    if(showLogs)
                        Debug.Log("tengo " + connections + " conexiones por individuo");
                    connections = 0;
                }
            }
        }

        GUI.color = originalCol;
        showLogs = false;
    }
    #endregion
}
