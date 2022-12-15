using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshReconstructor : MonoBehaviour
{
    public bool nextIterationNeeded = true;

    public Mesh ReconstructMeshUntillDone(Mesh mesh)
    {
        do
        {
            mesh = ReconstructMesh(mesh);
        } while (nextIterationNeeded);

        return mesh;
    }

    public Mesh ReconstructMesh(Mesh mesh)
    {
        //Debug.Log("RM 0");
        nextIterationNeeded = false;

        List<AffectedTriangle> affectedTris = new List<AffectedTriangle>();
        List<CriticalLine> criticalLines = new List<CriticalLine>();

        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        //Debug.Log("number of verts " + verts.Length);
        //Debug.Log("number of tris " + tris.Length / 3);


        //Add critical lines
        int index = 0;
        while (index < tris.Length)
        {
            int triStartI = index;
            int triI0 = tris[index++];
            int triI1 = tris[index++];
            int triI2 = tris[index++];

            Vector3 p0 = verts[triI0];
            Vector3 p1 = verts[triI1];
            Vector3 p2 = verts[triI2];

            float normalY = (Vector3.Cross(p1 - p0, p2 - p0)).normalized.y;

            //triangles of a rock surface can have a maximum Edge Length of 26cm while other surfaces should not have more than 10cm
            bool rockSurface = (normalY < 0.25f && normalY > -0.5f);

            float maxGroundEL = 0.13f;
            float maxRockEL = 0.50f;
            float twiceMaxAllowedEdgeLength = rockSurface ? maxRockEL : maxGroundEL;
            Vector3 l0 = p0 - p1;
            Vector3 l1 = p1 - p2;
            Vector3 l2 = p2 - p0;

            // lines marked as optional are only optional critical for the triangle. This means that the line split might be used by another non-optional critical triangle.
            if (l0.magnitude > maxGroundEL)
            {
                bool optional = (rockSurface && l0.magnitude < maxRockEL);
                criticalLines.Add(new CriticalLine(optional, triI0, triI1, triStartI));
                if (l0.magnitude > twiceMaxAllowedEdgeLength) nextIterationNeeded = true;
            }
            if (l1.magnitude > maxGroundEL)
            {
                bool optional = (rockSurface && l1.magnitude < maxRockEL);
                criticalLines.Add(new CriticalLine(optional, triI1, triI2, triStartI));
                if (l1.magnitude > twiceMaxAllowedEdgeLength) nextIterationNeeded = true;
            }
            if (l2.magnitude > maxGroundEL)
            {
                bool optional = (rockSurface && l2.magnitude < maxRockEL);
                criticalLines.Add(new CriticalLine(optional, triI2, triI0, triStartI));
                if (l2.magnitude > twiceMaxAllowedEdgeLength) nextIterationNeeded = true;
            }


        }
        //Debug.Log("number of critical lines: " + criticalLines.Count);

        //Debug.Log("RM 1");

        //What is done next?: For almost every line there is a twin line (same line belonging to another triangle)
        //This twins should be combined by:
        //removing one of the twins
        //and remove both twins if both are optional
        //also remove a line if it is optional and does not have a twin
        //add the removed twin's corresponding triangle to the left twin to combine them
        List<CriticalLine> criticalLinesLeft = new List<CriticalLine>();

        //these two lists are used to save access-/copy-performance in the innermost loop
        List<int> vertexIndicesOfLines0 = criticalLines.Select(c => c.vertIndex0).ToList();
        List<int> vertexIndicesOfLines1 = criticalLines.Select(c => c.vertIndex1).ToList();
        bool[] isLineRemoved = new bool[criticalLines.Count];

        //Debug.Log("RM 2");

        for (int i = 0; i < criticalLines.Count; i++)
        {

            if (isLineRemoved[i]) continue; //only check non-removed lines
            CriticalLine checkedLine = criticalLines[i];

            int twinIndex = -1;
            //CriticalLine twin;
            for (int j = i + 1; j < criticalLines.Count; j++)
            {
                if (isLineRemoved[j]) continue; //only compare non-removed lines
                if (checkedLine.vertIndex0 == vertexIndicesOfLines0[j])
                {
                    if (checkedLine.vertIndex1 == vertexIndicesOfLines1[j])
                    {
                        twinIndex = j;
                        isLineRemoved[j] = true;
                        //Debug.Log("remove twin line " + j);
                        break; //fitting twin was found, no need to keep searching
                    }
                }
                else if (checkedLine.vertIndex0 == vertexIndicesOfLines1[j])
                {
                    if (checkedLine.vertIndex1 == vertexIndicesOfLines0[j])
                    {
                        twinIndex = j;
                        isLineRemoved[j] = true;
                        //Debug.Log("remove twin line " + j);
                        break; //fitting twin was found, no need to keep searching
                    }
                }
            }

            if (checkedLine.optional)
            {
                if (twinIndex == -1)
                {

                    isLineRemoved[i] = true;
                    //Debug.Log("remove solo line " + i);
                    continue; //optional lines without twin get removed, optional lines with an optional twin get removed aswell
                }else if (criticalLines[twinIndex].optional)
                {
                    isLineRemoved[i] = true;
                    //Debug.Log("remove solo line because twin was optional aswell" + i);
                    continue; //optional lines without twin get removed, optional lines with an optional twin get removed aswell
                }
            }


            
            if (twinIndex == -1) continue;
            //add the second corresponding triangle to combine twins
            checkedLine.connectedTriIndex1 = criticalLines[twinIndex].connectedTriIndex0;
            checkedLine.doubleCorrespondence = true;
            criticalLines[i]= checkedLine;
        }


        //Debug.Log("RM 3");

        //clear removed lines from list
        for (int i = 0; i < criticalLines.Count; i++)
        {
            if (!isLineRemoved[i]) criticalLinesLeft.Add(criticalLines[i]);
        }
        criticalLines = criticalLinesLeft;

        //Debug.Log("number of critical lines after delete twins: " + criticalLines.Count);
        //Debug.Log("RM 4");

        //Add new Verticies by critical lines
        int vertsOriginalLength = verts.Length;
        List<Vector3> newVertecies = new List<Vector3>();
        foreach (CriticalLine c in criticalLines)
        {
            newVertecies.Add((verts[c.vertIndex0] + verts[c.vertIndex1]) / 2);
        }

        //foreach(Vector3 v in verts) //ONLY DEBUG DELETE LATER
        //{
        //    Debug.Log("v before" + v);
        //}
        List<Vector3> list = new List<Vector3>();
        list.AddRange(verts);
        list.AddRange(newVertecies);
        verts = list.ToArray();
        //foreach (Vector3 v in verts) //ONLY DEBUG DELETE LATER
        //{
        //    Debug.Log("v after" + v);
        //}

        //Debug.Log("verts after clVerts added: " + verts.Length);
        //Debug.Log("RM 5");

        //create triangles out of lines
        for (int i = 0; i < criticalLines.Count; i++)
        {
            CriticalLine checkedLine = criticalLines[i];
            //most lines have two corresponding triangles, thusthe two variables "existingTriIndex0" "existingTriIndex1"
            //if an "existingTriIndex" is bigger than 0, it was already saved as an affected triangle at the index value position and the line must be registered there.
            //if an "existingTriIndex" is -1, it was not saved yet and has to be saved as a new affected triangle.
            //if the "existingTriIndex1" is -2, the line only correspondes to one triangle pointed at by "existingTriIndex0"
            int existingTriIndex0 = -1;
            int existingTriIndex1 = -1;
            for (int j = 0; j < affectedTris.Count; j++)
            {
                if (checkedLine.connectedTriIndex0 == affectedTris[j].triIndex) existingTriIndex0 = j;
            }
            if (checkedLine.doubleCorrespondence)
            {
                for (int j = 0; j < affectedTris.Count; j++)
                {
                    if (checkedLine.connectedTriIndex1 == affectedTris[j].triIndex) existingTriIndex1 = j;
                }
            }
            else
            {
                existingTriIndex1 = -2;
            }

            //Hier werden je nach existingTriIndex neue Dreiecke erzeugt/ergänzt
            if (existingTriIndex0 == -1)
            {
                int triIndex0 = checkedLine.connectedTriIndex0;
                affectedTris.Add(new AffectedTriangle(triIndex0, tris[triIndex0], tris[triIndex0 + 1], tris[triIndex0 + 2], checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength));
            }
            else if (existingTriIndex0 > -1)
            {
                //WARNING: HERE IT STILL MUST BE CHECKET IF IT IS DONE TO THE ORIGINAL, OR IF JUST A COPY IS CREATED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //affectedTris[existingTriIndex0].AddLineToTriangle(checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength);
                AffectedTriangle affTri = affectedTris[existingTriIndex0];
                affTri.AddLineToTriangle(checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength);
                affectedTris[existingTriIndex0] = affTri;
            }

            if (existingTriIndex1 == -1)
            {
                int triIndex1 = checkedLine.connectedTriIndex1;
                affectedTris.Add(new AffectedTriangle(triIndex1, tris[triIndex1], tris[triIndex1 + 1], tris[triIndex1 + 2], checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength));
            }
            else if (existingTriIndex1 > -1)
            {
                //WARNING: HERE IT STILL MUST BE CHECKET IF IT IS DONE TO THE ORIGINAL, OR IF JUST A COPY IS CREATED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //affectedTris[existingTriIndex1].AddLineToTriangle(checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength);
                AffectedTriangle affTri = affectedTris[existingTriIndex1];
                affTri.AddLineToTriangle(checkedLine.vertIndex0, checkedLine.vertIndex1, i + vertsOriginalLength);
                affectedTris[existingTriIndex1] = affTri;
            }

        }


        //Debug.Log("RM 6");



        //create new triangles out of affected Triangles
        List<int> newTriangles = new List<int>();
        for (int i = 0; i < affectedTris.Count; i++)
        {
            AffectedTriangle tri = affectedTris[i];

            int vi0, vi1, vi2; //original verticies indices (order might be changed)
            int e0, e1, e2; //potentional extra verticies indices ()

            int startingPoint = -1;
            if (tri.extraDots == 1)
            {
                if (tri.index01 != -1) startingPoint = 0;
                else if (tri.index12 != -1) startingPoint = 1;
                else if (tri.index20 != -1) startingPoint = 2;
            }
            else if (tri.extraDots == 2)
            {
                if (tri.index01 == -1) startingPoint = 1;
                else if (tri.index12 == -1) startingPoint = 2;
                else if (tri.index20 == -1) startingPoint = 0;
            }
            else if (tri.extraDots == 3)
            {
                startingPoint = 0;
            }
            else
            {
                Debug.LogError("triangle" + tri.triIndex + " has a wrong number of extra Dots: " + tri.extraDots);
            }

            switch (startingPoint)
            {
                case 0:
                    vi0 = tri.vertIndex0;
                    vi1 = tri.vertIndex1;
                    vi2 = tri.vertIndex2;
                    e0 = tri.index01;
                    e1 = tri.index12;
                    e2 = tri.index20;
                    break;
                case 1:
                    vi0 = tri.vertIndex1;
                    vi1 = tri.vertIndex2;
                    vi2 = tri.vertIndex0;
                    e0 = tri.index12;
                    e1 = tri.index20;
                    e2 = tri.index01;
                    break;
                case 2:
                    vi0 = tri.vertIndex2;
                    vi1 = tri.vertIndex0;
                    vi2 = tri.vertIndex1;
                    e0 = tri.index20;
                    e1 = tri.index01;
                    e2 = tri.index12;
                    break;
                default:
                    Debug.LogError("triangle" + tri.triIndex + " has an invalid starting point: " + startingPoint);
                    vi0 = -1;
                    vi1 = -1;
                    vi2 = -1;
                    e0 = -1;
                    e1 = -1;
                    e2 = -1;
                    break;
            }
            //e0 += vertsOriginalLength;
            //e1 += vertsOriginalLength;
            //e2 += vertsOriginalLength;

            //Debug.Log("affectedTri" + i + "  has " + tri.extraDots + " extraDots");
            int[] AddedTris = new int[0];
            switch (tri.extraDots)
            {
                case 1:
                    AddedTris = new int[]
                    {
                        vi0,e0,vi2,
                        e0,vi1,vi2
                    };
                    break;
                case 2:
                    AddedTris = new int[]
                    {
                        vi0,e0,vi2,
                        e0,vi1,e1,
                        e1,vi2,e0
                    };
                    break;
                case 3:
                    AddedTris = new int[]
                    {
                        vi0,e0,e2,
                        e0,vi1,e1,
                        e1,vi2,e2,
                        e0,e1,e2
                    };
                    break;
            }
            newTriangles.AddRange(AddedTris);
        }


        //Debug.Log("RM 7");

        //mark triangles, that should be removed
        foreach (AffectedTriangle tri in affectedTris)
        {
            tris[tri.triIndex] = -1;
        }
        //copy non-removed triangles into new list
        List<int> combinedTriangleArray = new List<int>();
        for (int i = 0; i < tris.Length; i += 3)
        {
            if (tris[i] != -1)
            {
                combinedTriangleArray.Add(tris[i]);
                combinedTriangleArray.Add(tris[i + 1]);
                combinedTriangleArray.Add(tris[i + 2]);
            }
        }
        //add new generated triangles
        combinedTriangleArray.AddRange(newTriangles);


        //Debug.Log("RM 8");

        //apply
        mesh.vertices = verts;
        mesh.triangles = combinedTriangleArray.ToArray();

        //Debug.Log("number of verts afterwards" + verts.Length);
        //Debug.Log("number of tris afterwards" + combinedTriangleArray.Count / 3);




        return mesh;
    }



    struct AffectedTriangle
    {
        public int triIndex;

        //index of verticy
        public int vertIndex0;
        public int vertIndex1;
        public int vertIndex2;

        public int index01;
        public int index12;
        public int index20;

        public int extraDots;

        public AffectedTriangle(int triIndex, int vertIndex0, int vertIndex1, int vertIndex2, int lineFirstVI, int lineSecondVI, int lineIndex)
        {
            this.triIndex = triIndex;
            this.vertIndex0 = vertIndex0;
            this.vertIndex1 = vertIndex1;
            this.vertIndex2 = vertIndex2;

            extraDots = 0;
            index01 = -1;
            index12 = -1;
            index20 = -1;

            AddLineToTriangle(lineFirstVI, lineSecondVI, lineIndex);
        }

        public void AddLineToTriangle(int lineFirstVI, int lineSecondVI, int lineIndex)
        {
            //Debug.Log("AddlineToTriangle called by line " + lineIndex);
            int line = FindLineVarIdentifier(lineFirstVI, lineSecondVI);

            if (line == 0) index01 = lineIndex;
            else if (line == 1) index12 = lineIndex;
            else if (line == 2) index20 = lineIndex;
        }

        int FindLineVarIdentifier(int lineFirstVI, int lineSecondVI)
        {
            int line = -1;  // if it keeps on -1 something went wrong
                            // =0 is the line between vertex0 to vertex1
                            // =1 is the line between vertex1 to vertex2
                            // =2 is the line between vertex2 to vertex0
                            //now put lineIndex onto the correct line;
            if (lineFirstVI == vertIndex0)
            {
                if (lineSecondVI == vertIndex1)
                {
                    line = 0;
                }
                else if (lineSecondVI == vertIndex2)
                {
                    line = 2;
                }
            }
            else if (lineFirstVI == vertIndex1)
            {
                if (lineSecondVI == vertIndex0)
                {
                    line = 0;
                }
                else if (lineSecondVI == vertIndex2)
                {
                    line = 1;
                }
            }
            else if (lineFirstVI == vertIndex2)
            {
                if (lineSecondVI == vertIndex1)
                {
                    line = 1;
                }
                else if (lineSecondVI == vertIndex0)
                {
                    line = 2;
                }
            }

            if (line == -1)
            {
                Debug.LogError("Something went wrong at Triangle " + triIndex + ".  v0=" + vertIndex0 + "v1=" + vertIndex0 + "v2=" + vertIndex0 + "  line verticies are " + lineFirstVI + " and " + lineSecondVI);
            }
            else
            {
                
                extraDots++;
                //Debug.Log("added dot. dots are now: " + extraDots);
            }


            return line;
        }
    }

    struct CriticalLine
    {
        public bool optional;
        public bool removed;
        public bool doubleCorrespondence;

        public int vertIndex0;
        public int vertIndex1;

        public int connectedTriIndex0;
        public int connectedTriIndex1;

        public CriticalLine(bool optional, int vertIndex0, int vertIndex1, int connectedTriIndex0)
        {
            this.optional = optional;
            this.vertIndex0 = vertIndex0;
            this.vertIndex1 = vertIndex1;
            this.connectedTriIndex0 = connectedTriIndex0;

            this.connectedTriIndex1 = -1;
            this.removed = false;
            this.doubleCorrespondence = false;

            //if (optional) Debug.Log("optional line created");
        }
    }
}
