Imports System.IO
Imports System.Text

Public Class cBSP

#Region "Variables"

    Private header As Header

    Private edges As List(Of UShort())

    Private vertices As Vec3()

    Private originalFaces As Face()

    Private faces As Face()

    Private planes As Plane()

    Private brushes As Brush()

    Private brushsides As Brushside()

    Private nodes As Node()

    Private leafs As Leaf()

    Private surfedges As Integer()

    Private textureInfo As SurfFlag()

    Private entityBuffer As String

#End Region

    Public Sub New()
        'Nothing
    End Sub

    Public Sub New(ByVal stream As Stream)
        Load(stream)
    End Sub

    Public Sub New(ByVal filePath As String)
        Using stream As FileStream = File.OpenRead(filePath)
            Load(stream)
        End Using
    End Sub

    Public Sub Load(ByVal filePath As String)
        Using stream As FileStream = File.OpenRead(filePath)
            Load(stream)
        End Using
    End Sub

    Private Sub Load(ByVal stream As Stream)
        Me.header = GetHeader(stream)
        Me.edges = GetEdges(stream)
        Me.vertices = GetVertices(stream)
        Me.originalFaces = GetOriginalFaces(stream)
        Me.faces = GetFaces(stream)
        Me.planes = GetPlanes(stream)
        Me.surfedges = GetSurfedges(stream)
        Me.textureInfo = GetTextureInfo(stream)
        Me.brushes = GetBrushes(stream)
        Me.brushsides = GetBrushsides(stream)
        Me.entityBuffer = GetEntities(stream)
        Me.nodes = GetNodes(stream)
        Me.leafs = GetLeafs(stream)
    End Sub

#Region "Methods"

    Private Function GetHeader(ByVal stream As Stream) As Header
        Dim header As Header = New Header
        header.ident = UtilityReader.ReadInt(stream)

        If header.ident = CInt(Math.Truncate(AscW("V"c) + (AscW("B"c) << 8) + (AscW("S"c) << 16) + (AscW("P"c) << 24))) Then
            UtilityReader.bigEndian = False
        Else
            UtilityReader.bigEndian = True
        End If

        header.version = UtilityReader.ReadInt(stream)
        header.lumps = New Lump(63) {}
        For i As Integer = 0 To header.lumps.Length - 1
            header.lumps(i) = New Lump()
            header.lumps(i).type = CType(i, LumpType)
            header.lumps(i).offset = UtilityReader.ReadInt(stream)
            header.lumps(i).length = UtilityReader.ReadInt(stream)
            header.lumps(i).version = UtilityReader.ReadInt(stream)
            header.lumps(i).fourCC = UtilityReader.ReadInt(stream)
        Next

        header.mapRevision = UtilityReader.ReadInt(stream)
        Return header
    End Function

    Private Function GetEntities(ByVal stream As Stream) As String
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_ENTITIES))
        stream.Position = lump.offset
        Dim data As Byte() = UtilityReader.ReadBytes(stream, lump.length)
        Return Encoding.ASCII.GetString(data)
    End Function

    Private Function GetEdges(ByVal stream As Stream) As List(Of UShort())
        Dim edges As List(Of UShort()) = New List(Of UShort())()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_EDGES))
        stream.Position = lump.offset
        For i As Integer = 0 To (lump.length / 2) / 2 - 1
            Dim edge As UShort() = New UShort(1) {}
            edge(0) = UtilityReader.ReadUShort(stream)
            edge(1) = UtilityReader.ReadUShort(stream)
            edges.Add(edge)
        Next

        Return edges
    End Function

    Private Function GetVertices(ByVal stream As Stream) As Vec3()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_VERTEXES))
        stream.Position = lump.offset
        Dim vertices As Vec3() = New Vec3((lump.length / 3) / 4 - 1) {}
        For i As Integer = 0 To vertices.Length - 1
            vertices(i) = New Vec3()
            vertices(i).X = UtilityReader.ReadFloat(stream)
            vertices(i).Y = UtilityReader.ReadFloat(stream)
            vertices(i).Z = UtilityReader.ReadFloat(stream)
        Next

        Return vertices
    End Function

    Private Function GetOriginalFaces(ByVal stream As Stream) As Face()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_ORIGINALFACES))
        stream.Position = lump.offset
        Dim faces As Face() = New Face(lump.length / 56 - 1) {}
        For i As Integer = 0 To faces.Length - 1
            faces(i) = New Face()
            faces(i).planeNumber = UtilityReader.ReadUShort(stream)
            faces(i).side = UtilityReader.ReadByte(stream)
            faces(i).onNode = UtilityReader.ReadByte(stream)
            faces(i).firstEdge = UtilityReader.ReadInt(stream)
            faces(i).numEdges = UtilityReader.ReadShort(stream)
            faces(i).texinfo = UtilityReader.ReadShort(stream)
            faces(i).dispinfo = UtilityReader.ReadShort(stream)
            faces(i).surfaceFogVolumeID = UtilityReader.ReadShort(stream)
            faces(i).styles = New Byte(3) {}
            faces(i).styles(0) = UtilityReader.ReadByte(stream)
            faces(i).styles(1) = UtilityReader.ReadByte(stream)
            faces(i).styles(2) = UtilityReader.ReadByte(stream)
            faces(i).styles(3) = UtilityReader.ReadByte(stream)
            faces(i).lightOffset = UtilityReader.ReadInt(stream)
            faces(i).area = UtilityReader.ReadFloat(stream)
            faces(i).LightmapTextureMinsInLuxels = New Integer(1) {}
            faces(i).LightmapTextureMinsInLuxels(0) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureMinsInLuxels(1) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureSizeInLuxels = New Integer(1) {}
            faces(i).LightmapTextureSizeInLuxels(0) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureSizeInLuxels(1) = UtilityReader.ReadInt(stream)
            faces(i).originalFace = UtilityReader.ReadInt(stream)
            faces(i).numPrims = UtilityReader.ReadUShort(stream)
            faces(i).firstPrimID = UtilityReader.ReadUShort(stream)
            faces(i).smoothingGroups = UtilityReader.ReadUInt(stream)
        Next

        Return faces
    End Function

    Private Function GetFaces(ByVal stream As Stream) As Face()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_FACES))
        stream.Position = lump.offset
        Dim faces As Face() = New Face(lump.length / 56 - 1) {}
        For i As Integer = 0 To faces.Length - 1
            faces(i) = New Face()
            faces(i).planeNumber = UtilityReader.ReadUShort(stream)
            faces(i).side = UtilityReader.ReadByte(stream)
            faces(i).onNode = UtilityReader.ReadByte(stream)
            faces(i).firstEdge = UtilityReader.ReadInt(stream)
            faces(i).numEdges = UtilityReader.ReadShort(stream)
            faces(i).texinfo = UtilityReader.ReadShort(stream)
            faces(i).dispinfo = UtilityReader.ReadShort(stream)
            faces(i).surfaceFogVolumeID = UtilityReader.ReadShort(stream)
            faces(i).styles = New Byte(3) {}
            faces(i).styles(0) = UtilityReader.ReadByte(stream)
            faces(i).styles(1) = UtilityReader.ReadByte(stream)
            faces(i).styles(2) = UtilityReader.ReadByte(stream)
            faces(i).styles(3) = UtilityReader.ReadByte(stream)
            faces(i).lightOffset = UtilityReader.ReadInt(stream)
            faces(i).area = UtilityReader.ReadFloat(stream)
            faces(i).LightmapTextureMinsInLuxels = New Integer(1) {}
            faces(i).LightmapTextureMinsInLuxels(0) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureMinsInLuxels(1) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureSizeInLuxels = New Integer(1) {}
            faces(i).LightmapTextureSizeInLuxels(0) = UtilityReader.ReadInt(stream)
            faces(i).LightmapTextureSizeInLuxels(1) = UtilityReader.ReadInt(stream)
            faces(i).originalFace = UtilityReader.ReadInt(stream)
            faces(i).numPrims = UtilityReader.ReadUShort(stream)
            faces(i).firstPrimID = UtilityReader.ReadUShort(stream)
            faces(i).smoothingGroups = UtilityReader.ReadUInt(stream)
        Next

        Return faces
    End Function

    Private Function GetPlanes(ByVal stream As Stream) As Plane()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_PLANES))
        Dim planes As Plane() = New Plane(lump.length / 20 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To planes.Length - 1
            planes(i) = New Plane()
            Dim normal As Vec3 = New Vec3()
            normal.X = UtilityReader.ReadFloat(stream)
            normal.Y = UtilityReader.ReadFloat(stream)
            normal.Z = UtilityReader.ReadFloat(stream)
            planes(i).normal = normal
            planes(i).distance = UtilityReader.ReadFloat(stream)
            planes(i).type = UtilityReader.ReadInt(stream)
        Next

        Return planes
    End Function

    Private Function GetBrushes(ByVal stream As Stream) As Brush()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_BRUSHES))
        Dim brushes As Brush() = New Brush(lump.length / 12 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To brushes.Length - 1
            brushes(i) = New Brush()
            brushes(i).firstside = UtilityReader.ReadInt(stream)
            brushes(i).numsides = UtilityReader.ReadInt(stream)
            brushes(i).contents = CType(UtilityReader.ReadInt(stream), ContentsFlag)
        Next

        Return brushes
    End Function

    Private Function GetBrushsides(ByVal stream As Stream) As Brushside()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_BRUSHES))
        Dim brushsides As Brushside() = New Brushside(lump.length / 8 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To brushsides.Length - 1
            brushsides(i) = New Brushside()
            brushsides(i).planenum = UtilityReader.ReadUShort(stream)
            brushsides(i).texinfo = UtilityReader.ReadShort(stream)
            brushsides(i).dispinfo = UtilityReader.ReadShort(stream)
            brushsides(i).bevel = UtilityReader.ReadShort(stream)
        Next

        Return brushsides
    End Function

    Private Function GetSurfedges(ByVal stream As Stream) As Integer()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_SURFEDGES))
        Dim surfedges As Integer() = New Integer(lump.length / 4 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To lump.length / 4 - 1
            surfedges(i) = UtilityReader.ReadInt(stream)
        Next

        Return surfedges
    End Function

    Private Function GetTextureInfo(ByVal stream As Stream) As SurfFlag()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_TEXINFO))
        Dim textureData As SurfFlag() = New SurfFlag(lump.length / 72 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To textureData.Length - 1
            stream.Position += 64
            textureData(i) = CType(UtilityReader.ReadInt(stream), SurfFlag)
            stream.Position += 4
        Next

        Return textureData
    End Function

    Private Function GetNodes(ByVal stream As Stream) As Node()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_NODES))
        Dim nodesData As Node() = New Node(lump.length / 32 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To nodesData.Length - 1
            nodesData(i) = New Node()
            nodesData(i).planenum = UtilityReader.ReadInt(stream)
            nodesData(i).children = New Integer(1) {}
            nodesData(i).children(0) = UtilityReader.ReadInt(stream)
            nodesData(i).children(1) = UtilityReader.ReadInt(stream)
            nodesData(i).mins = New Short(2) {}
            nodesData(i).mins(0) = UtilityReader.ReadShort(stream)
            nodesData(i).mins(1) = UtilityReader.ReadShort(stream)
            nodesData(i).mins(2) = UtilityReader.ReadShort(stream)
            nodesData(i).maxs = New Short(2) {}
            nodesData(i).maxs(0) = UtilityReader.ReadShort(stream)
            nodesData(i).maxs(1) = UtilityReader.ReadShort(stream)
            nodesData(i).maxs(2) = UtilityReader.ReadShort(stream)
            nodesData(i).firstface = UtilityReader.ReadUShort(stream)
            nodesData(i).numfaces = UtilityReader.ReadUShort(stream)
            nodesData(i).area = UtilityReader.ReadShort(stream)
            nodesData(i).paddding = UtilityReader.ReadShort(stream)
        Next

        Return nodesData
    End Function

    Private Function GetLeafs(ByVal stream As Stream) As Leaf()
        Dim lump As Lump = header.lumps(CInt(LumpType.LUMP_LEAFS))
        Dim leafData As Leaf() = New Leaf(lump.length / 56 - 1) {}
        stream.Position = lump.offset
        For i As Integer = 0 To leafData.Length - 1
            leafData(i) = New Leaf()
            leafData(i).contents = CType(UtilityReader.ReadInt(stream), ContentsFlag)
            leafData(i).cluster = UtilityReader.ReadShort(stream)
            leafData(i).area = UtilityReader.ReadShort(stream)
            leafData(i).flags = UtilityReader.ReadShort(stream)
            leafData(i).mins = New Short(2) {}
            leafData(i).mins(0) = UtilityReader.ReadShort(stream)
            leafData(i).mins(1) = UtilityReader.ReadShort(stream)
            leafData(i).mins(2) = UtilityReader.ReadShort(stream)
            leafData(i).maxs = New Short(2) {}
            leafData(i).maxs(0) = UtilityReader.ReadShort(stream)
            leafData(i).maxs(1) = UtilityReader.ReadShort(stream)
            leafData(i).maxs(2) = UtilityReader.ReadShort(stream)
            leafData(i).firstleafface = UtilityReader.ReadUShort(stream)
            leafData(i).numleaffaces = UtilityReader.ReadUShort(stream)
            leafData(i).firstleafbrush = UtilityReader.ReadUShort(stream)
            leafData(i).numleafbrushes = UtilityReader.ReadUShort(stream)
            leafData(i).leafWaterDataID = UtilityReader.ReadShort(stream)
        Next

        Return leafData
    End Function

#End Region
End Class

Class UtilityReader

    Public Shared bigEndian As Boolean = False


    Public Shared Function ReadByte(ByVal stream As Stream) As Byte
        Dim buffer As Byte() = ReadBytes(stream, 1)
        Return buffer(0)
    End Function

    Public Shared Function ReadShort(ByVal stream As Stream) As Short
        Dim buffer As Byte() = ReadBytes(stream, 2)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToInt16(buffer, 0)
    End Function

    Public Shared Function ReadUShort(ByVal stream As Stream) As UShort
        Dim buffer As Byte() = ReadBytes(stream, 2)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToUInt16(buffer, 0)
    End Function

    Public Shared Function ReadInt(ByVal stream As Stream) As Integer
        Dim buffer As Byte() = ReadBytes(stream, 4)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToInt32(buffer, 0)
    End Function

    Public Shared Function ReadUInt(ByVal stream As Stream) As UInteger
        Dim buffer As Byte() = ReadBytes(stream, 4)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToUInt32(buffer, 0)
    End Function

    Public Shared Function ReadLong(ByVal stream As Stream) As Long
        Dim buffer As Byte() = ReadBytes(stream, 8)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToInt64(buffer, 0)
    End Function

    Public Shared Function ReadFloat(ByVal stream As Stream) As Single
        Dim buffer As Byte() = ReadBytes(stream, 4)
        If bigEndian Then buffer.Reverse()
        Return BitConverter.ToSingle(buffer, 0)
    End Function

    Public Shared Function ReadBytes(ByVal stream As Stream, ByVal count As Integer) As Byte()
        Dim buffer As Byte() = New Byte(count - 1) {}
        stream.Read(buffer, 0, count)
        Return buffer
    End Function

End Class
