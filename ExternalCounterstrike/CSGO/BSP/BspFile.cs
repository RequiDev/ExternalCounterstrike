using ExternalCounterstrike.ConsoleSystem;
using ExternalCounterstrike.CSGO.BSP.Enums;
using ExternalCounterstrike.CSGO.BSP.Structs;
using ExternalCounterstrike.CSGO.Structs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExternalCounterstrike.CSGO.BSP
{
    internal class BspFile
    {
        #region VARIABLES
        private Header header;
        private List<ushort[]> edges;
        private Vector3D[] vertices;
        private Face[] originalFaces;
        private Face[] faces;
        private Plane[] planes;
        private Brush[] brushes;
        private Brushside[] brushsides;
        private Node[] nodes;
        private Leaf[] leafs;
        private int[] surfedges;
        private SurfFlag[] textureInfo;
        private string entityBuffer;
        #endregion

        #region PROPERTIES
        public Header Header { get { return header; } }
        public List<ushort[]> Edges { get { return edges; } }
        public Vector3D[] Vertices { get { return vertices; } }
        public Face[] OriginalFaces { get { return originalFaces; } }
        public Face[] Faces { get { return faces; } }
        public Plane[] Planes { get { return planes; } }
        public Brush[] Brushes { get { return brushes; } }
        public Brushside[] Brushsides { get { return brushsides; } }
        public Node[] Nodes { get { return nodes; } }
        public Leaf[] Leafs { get { return leafs; } }
        public int[] Surfedges { get { return surfedges; } }
        public SurfFlag[] TextureInfo { get { return textureInfo; } }
        public string EntityBuffer { get { return entityBuffer; } }
        public bool LoadedData = false;
        #endregion

        #region CONSTRUCTORS
        public BspFile(Stream stream)
        {
            Load(stream);
        }
        public BspFile(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                Load(stream);
            }
            Console.WriteSuccess("\nLoaded " + filePath.Split('\\').Last());
            Console.WriteCommandLine();
        }
        #endregion

        #region METHODS - LOAD
        private void Load(Stream stream)
        {
            this.header = GetHeader(stream);
            this.edges = GetEdges(stream);
            this.vertices = GetVertices(stream);
            this.originalFaces = GetOriginalFaces(stream);
            this.faces = GetFaces(stream);
            this.planes = GetPlanes(stream);
            this.surfedges = GetSurfedges(stream);
            this.textureInfo = GetTextureInfo(stream);
            this.brushes = GetBrushes(stream);
            this.brushsides = GetBrushsides(stream);
            this.entityBuffer = GetEntities(stream);
            this.nodes = GetNodes(stream);
            this.leafs = GetLeafs(stream);
            LoadedData = true;
            //LoadWorld();
        }
        private string GetEntities(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_ENTITIES];
            stream.Position = lump.offset;
            byte[] data = UtilityReader.ReadBytes(stream, lump.length);
            return System.Text.Encoding.ASCII.GetString(data);
        }
        private Header GetHeader(Stream stream)
        {
            Header header = new Header();
            header.ident = UtilityReader.ReadInt(stream);

            if (header.ident == (int)('V' + ('B' << 8) + ('S' << 16) + ('P' << 24)))
                UtilityReader.BigEndian = false;
            else
                UtilityReader.BigEndian = true;

            header.version = UtilityReader.ReadInt(stream);
            header.lumps = new Lump[64];
            for (int i = 0; i < header.lumps.Length; i++)
            {
                header.lumps[i] = new Lump();
                header.lumps[i].type = (LumpType)i;
                header.lumps[i].offset = UtilityReader.ReadInt(stream);
                header.lumps[i].length = UtilityReader.ReadInt(stream);
                header.lumps[i].version = UtilityReader.ReadInt(stream);
                header.lumps[i].fourCC = UtilityReader.ReadInt(stream);
            }
            header.mapRevision = UtilityReader.ReadInt(stream);
            return header;
        }
        private List<ushort[]> GetEdges(Stream stream)
        {
            List<ushort[]> edges = new List<ushort[]>();
            Lump lump = header.lumps[(int)LumpType.LUMP_EDGES];
            stream.Position = lump.offset;

            for (int i = 0; i < (lump.length / 2) / 2; i++)
            {
                ushort[] edge = new ushort[2];
                edge[0] = UtilityReader.ReadUShort(stream);
                edge[1] = UtilityReader.ReadUShort(stream);
                edges.Add(edge);
            }

            return edges;
        }
        private Vector3D[] GetVertices(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_VERTEXES];
            stream.Position = lump.offset;
            Vector3D[] vertices = new Vector3D[(lump.length / 3) / 4];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3D();
                vertices[i].X = UtilityReader.ReadFloat(stream);
                vertices[i].Y = UtilityReader.ReadFloat(stream);
                vertices[i].Z = UtilityReader.ReadFloat(stream);
            }

            return vertices;
        }
        private Face[] GetOriginalFaces(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_ORIGINALFACES];
            stream.Position = lump.offset;
            Face[] faces = new Face[lump.length / 56];

            for (int i = 0; i < faces.Length; i++)
            {
                faces[i] = new Face();
                faces[i].planeNumber = UtilityReader.ReadUShort(stream);
                faces[i].side = UtilityReader.ReadByte(stream);
                faces[i].onNode = UtilityReader.ReadByte(stream);
                faces[i].firstEdge = UtilityReader.ReadInt(stream);
                faces[i].numEdges = UtilityReader.ReadShort(stream);
                faces[i].texinfo = UtilityReader.ReadShort(stream);
                faces[i].dispinfo = UtilityReader.ReadShort(stream);
                faces[i].surfaceFogVolumeID = UtilityReader.ReadShort(stream);
                faces[i].styles = new byte[4];
                faces[i].styles[0] = UtilityReader.ReadByte(stream);
                faces[i].styles[1] = UtilityReader.ReadByte(stream);
                faces[i].styles[2] = UtilityReader.ReadByte(stream);
                faces[i].styles[3] = UtilityReader.ReadByte(stream);
                faces[i].lightOffset = UtilityReader.ReadInt(stream);
                faces[i].area = UtilityReader.ReadFloat(stream);
                faces[i].LightmapTextureMinsInLuxels = new int[2];
                faces[i].LightmapTextureMinsInLuxels[0] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureMinsInLuxels[1] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureSizeInLuxels = new int[2];
                faces[i].LightmapTextureSizeInLuxels[0] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureSizeInLuxels[1] = UtilityReader.ReadInt(stream);
                faces[i].originalFace = UtilityReader.ReadInt(stream);
                faces[i].numPrims = UtilityReader.ReadUShort(stream);
                faces[i].firstPrimID = UtilityReader.ReadUShort(stream);
                faces[i].smoothingGroups = UtilityReader.ReadUInt(stream);
            }

            return faces;
        }
        private Face[] GetFaces(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_FACES];
            stream.Position = lump.offset;
            Face[] faces = new Face[lump.length / 56];

            for (int i = 0; i < faces.Length; i++)
            {
                faces[i] = new Face();
                faces[i].planeNumber = UtilityReader.ReadUShort(stream);
                faces[i].side = UtilityReader.ReadByte(stream);
                faces[i].onNode = UtilityReader.ReadByte(stream);
                faces[i].firstEdge = UtilityReader.ReadInt(stream);
                faces[i].numEdges = UtilityReader.ReadShort(stream);
                faces[i].texinfo = UtilityReader.ReadShort(stream);
                faces[i].dispinfo = UtilityReader.ReadShort(stream);
                faces[i].surfaceFogVolumeID = UtilityReader.ReadShort(stream);
                faces[i].styles = new byte[4];
                faces[i].styles[0] = UtilityReader.ReadByte(stream);
                faces[i].styles[1] = UtilityReader.ReadByte(stream);
                faces[i].styles[2] = UtilityReader.ReadByte(stream);
                faces[i].styles[3] = UtilityReader.ReadByte(stream);
                faces[i].lightOffset = UtilityReader.ReadInt(stream);
                faces[i].area = UtilityReader.ReadFloat(stream);
                faces[i].LightmapTextureMinsInLuxels = new int[2];
                faces[i].LightmapTextureMinsInLuxels[0] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureMinsInLuxels[1] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureSizeInLuxels = new int[2];
                faces[i].LightmapTextureSizeInLuxels[0] = UtilityReader.ReadInt(stream);
                faces[i].LightmapTextureSizeInLuxels[1] = UtilityReader.ReadInt(stream);
                faces[i].originalFace = UtilityReader.ReadInt(stream);
                faces[i].numPrims = UtilityReader.ReadUShort(stream);
                faces[i].firstPrimID = UtilityReader.ReadUShort(stream);
                faces[i].smoothingGroups = UtilityReader.ReadUInt(stream);
            }

            return faces;
        }
        private Plane[] GetPlanes(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_PLANES];
            Plane[] planes = new Plane[lump.length / 20];
            stream.Position = lump.offset;

            for (int i = 0; i < planes.Length; i++)
            {
                planes[i] = new Plane();

                Vector3D normal = new Vector3D();
                normal.X = UtilityReader.ReadFloat(stream);
                normal.Y = UtilityReader.ReadFloat(stream);
                normal.Z = UtilityReader.ReadFloat(stream);

                planes[i].normal = normal;
                planes[i].distance = UtilityReader.ReadFloat(stream);
                planes[i].type = UtilityReader.ReadInt(stream);
            }

            return planes;
        }
        private Brush[] GetBrushes(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_BRUSHES];
            Brush[] brushes = new Brush[lump.length / 12];
            stream.Position = lump.offset;

            for (int i = 0; i < brushes.Length; i++)
            {
                brushes[i] = new Brush();

                brushes[i].firstside = UtilityReader.ReadInt(stream);
                brushes[i].numsides = UtilityReader.ReadInt(stream);
                brushes[i].contents = (ContentsFlag)UtilityReader.ReadInt(stream);
            }

            return brushes;
        }
        private Brushside[] GetBrushsides(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_BRUSHES];
            Brushside[] brushsides = new Brushside[lump.length / 8];
            stream.Position = lump.offset;

            for (int i = 0; i < brushsides.Length; i++)
            {
                brushsides[i] = new Brushside();

                brushsides[i].planenum = UtilityReader.ReadUShort(stream);
                brushsides[i].texinfo = UtilityReader.ReadShort(stream);
                brushsides[i].dispinfo = UtilityReader.ReadShort(stream);
                brushsides[i].bevel = UtilityReader.ReadShort(stream);
            }

            return brushsides;
        }
        private int[] GetSurfedges(Stream stream)
        {

            Lump lump = header.lumps[(int)LumpType.LUMP_SURFEDGES];
            int[] surfedges = new int[lump.length / 4];
            stream.Position = lump.offset;

            for (int i = 0; i < lump.length / 4; i++)
            {
                surfedges[i] = UtilityReader.ReadInt(stream);
            }

            return surfedges;
        }
        private SurfFlag[] GetTextureInfo(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_TEXINFO];
            SurfFlag[] textureData = new SurfFlag[lump.length / 72];
            stream.Position = lump.offset;

            for (int i = 0; i < textureData.Length; i++)
            {
                stream.Position += 64;
                textureData[i] = (SurfFlag)UtilityReader.ReadInt(stream);
                stream.Position += 4;
            }

            return textureData;
        }
        private Node[] GetNodes(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_NODES];
            Node[] nodesData = new Node[lump.length / 32];
            stream.Position = lump.offset;

            for (int i = 0; i < nodesData.Length; i++)
            {
                nodesData[i] = new Node();

                nodesData[i].planenum = UtilityReader.ReadInt(stream);

                nodesData[i].children = new int[2];
                nodesData[i].children[0] = UtilityReader.ReadInt(stream);
                nodesData[i].children[1] = UtilityReader.ReadInt(stream);

                nodesData[i].mins = new short[3];
                nodesData[i].mins[0] = UtilityReader.ReadShort(stream);
                nodesData[i].mins[1] = UtilityReader.ReadShort(stream);
                nodesData[i].mins[2] = UtilityReader.ReadShort(stream);

                nodesData[i].maxs = new short[3];
                nodesData[i].maxs[0] = UtilityReader.ReadShort(stream);
                nodesData[i].maxs[1] = UtilityReader.ReadShort(stream);
                nodesData[i].maxs[2] = UtilityReader.ReadShort(stream);

                nodesData[i].firstface = UtilityReader.ReadUShort(stream);
                nodesData[i].numfaces = UtilityReader.ReadUShort(stream);
                nodesData[i].area = UtilityReader.ReadShort(stream);
                nodesData[i].paddding = UtilityReader.ReadShort(stream);
            }

            return nodesData;
        }
        private Leaf[] GetLeafs(Stream stream)
        {
            Lump lump = header.lumps[(int)LumpType.LUMP_LEAFS];
            Leaf[] leafData = new Leaf[lump.length / 56];
            stream.Position = lump.offset;

            for (int i = 0; i < leafData.Length; i++)
            {
                leafData[i] = new Leaf();

                leafData[i].contents = (ContentsFlag)UtilityReader.ReadInt(stream);
                leafData[i].cluster = UtilityReader.ReadShort(stream);
                leafData[i].area = UtilityReader.ReadShort(stream);
                leafData[i].flags = UtilityReader.ReadShort(stream);

                leafData[i].mins = new short[3];
                leafData[i].mins[0] = UtilityReader.ReadShort(stream);
                leafData[i].mins[1] = UtilityReader.ReadShort(stream);
                leafData[i].mins[2] = UtilityReader.ReadShort(stream);

                leafData[i].maxs = new short[3];
                leafData[i].maxs[0] = UtilityReader.ReadShort(stream);
                leafData[i].maxs[1] = UtilityReader.ReadShort(stream);
                leafData[i].maxs[2] = UtilityReader.ReadShort(stream);

                leafData[i].firstleafface = UtilityReader.ReadUShort(stream);
                leafData[i].numleaffaces = UtilityReader.ReadUShort(stream);
                leafData[i].firstleafbrush = UtilityReader.ReadUShort(stream);
                leafData[i].numleafbrushes = UtilityReader.ReadUShort(stream);
                leafData[i].leafWaterDataID = UtilityReader.ReadShort(stream);
            }

            return leafData;
        }
        #endregion

        #region METHODS - VISIBILITY
        public Leaf GetLeafForPoint(Vector3D point)
        {
            int node = 0;

            Node pNode;
            Plane pPlane;

            float d = 0.0f;

            while (node >= 0)
            {
                pNode = nodes[node];
                pPlane = planes[pNode.planenum];

                d = Vector3D.Dot(point, pPlane.normal) - pPlane.distance;

                if (d > 0)
                {
                    node = pNode.children[0];
                }
                else
                {
                    node = pNode.children[1];
                }
            }

            return (
                (-node - 1) >= 0 && -node - 1 < leafs.Length ?
                leafs[-node - 1] :
                new Leaf() { area = -1, contents = ContentsFlag.CONTENTS_EMPTY }
            );
        }
        public bool IsVisible(Vector3D start, Vector3D end)
        {
            Vector3D vDirection = end - start;
            Vector3D vPoint = start;

            int iStepCount = (int)vDirection.Length();

            vDirection /= iStepCount;

            Leaf pLeaf = new Leaf() { area = -1 };

            while (iStepCount > 0)
            {
                vPoint += vDirection;

                pLeaf = GetLeafForPoint(vPoint);

                if (pLeaf.area != -1)
                {
                    if (
                        (pLeaf.contents & ContentsFlag.CONTENTS_SOLID) == ContentsFlag.CONTENTS_SOLID
                        || (pLeaf.contents & ContentsFlag.CONTENTS_DETAIL) == ContentsFlag.CONTENTS_DETAIL
                        )
                    {
                        break;
                    }
                }

                iStepCount--;
            }
            return (pLeaf.contents & ContentsFlag.CONTENTS_SOLID) != ContentsFlag.CONTENTS_SOLID;
        }
        #endregion
    }
}
