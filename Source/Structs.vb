Public Structure Vec3

    Public x As Single
    Public y As Single
    Public z As Single

    Public Function Length()
        If x < 0 Then x *= -1
        If y < 0 Then y *= -1
        If z < 0 Then z *= -1
        Return Math.Sqrt(x * x + y * y + z * y)
    End Function

    Public Overrides Function ToString() As String
        Return (x & " " & y & " " & z)
    End Function

    Public Sub New(_x As Single, _y As Single, _z As Single)
        x = _x
        y = _y
        z = _z
    End Sub

    Public Sub New(ByVal xyz As Single)
        x = xyz
        y = xyz
        z = xyz
    End Sub

    Public Function Dot(ByVal vec1 As Vec3, ByVal vec2 As Vec3) As Single
        Return ((vec1.x * vec2.x) + (vec1.y * vec2.y) + (vec1.z * vec2.z))
    End Function

    Public Shared Operator -(vec1 As Vec3, vec2 As Vec3) As Vec3
        Return New Vec3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z)
    End Operator

    Public Shared Operator +(vec1 As Vec3, vec2 As Vec3) As Vec3
        Return New Vec3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z)
    End Operator

    Public Shared Operator *(vec1 As Vec3, vec2 As Vec3) As Vec3
        Return New Vec3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z)
    End Operator

    Public Shared Operator *(vec1 As Vec3, multiplier As Single) As Vec3
        Return New Vec3(vec1.x * multiplier, vec1.y * multiplier, vec1.z * multiplier)
    End Operator

    Public Shared Operator <>(vec1 As Vec3, vec2 As Vec3) As Boolean
        If vec1.x <> vec2.x Or vec1.y <> vec2.y Or vec1.z <> vec2.z Then
            Return True
        End If
        Return False
    End Operator

    Public Shared Operator =(vec1 As Vec3, vec2 As Vec3) As Boolean
        If vec1.x = vec2.x And vec1.y = vec2.y And vec1.z = vec2.z Then
            Return True
        End If
        Return False
    End Operator

    Public Shared Operator /(vec1 As Vec3, vec2 As Vec3) As Vec3
        Return New Vec3(vec1.x / vec2.x, vec1.y / vec2.y, vec1.z / vec2.z)
    End Operator

    Public Shared Operator /(vec1 As Vec3, divisor As Single) As Vec3
        Return New Vec3(vec1.x / divisor, vec1.y / divisor, vec1.z / divisor)
    End Operator
End Structure

Public Structure Brush

    Public firstside, numsides As Integer

    Public contents As ContentsFlag

End Structure

Public Structure Brushside

    Public planenum As UShort

    Public texinfo, dispinfo, bevel As Short

End Structure

Public Structure Face

    Public planeNumber As UShort

    Public side As Byte

    Public onNode As Byte

    Public firstEdge As Integer

    Public numEdges As Short

    Public texinfo As Short

    Public dispinfo As Short

    Public surfaceFogVolumeID As Short

    Public styles As Byte()

    Public lightOffset As Integer

    Public area As Single

    Public LightmapTextureMinsInLuxels As Integer()

    Public LightmapTextureSizeInLuxels As Integer()

    Public originalFace As Integer

    Public numPrims As UShort

    Public firstPrimID As UShort

    Public smoothingGroups As UInteger

End Structure

Public Structure Header

    Public ident As Integer

    Public version As Integer

    Public lumps As Lump()

    Public mapRevision As Integer

End Structure


Public Structure Leaf

    Sub New(ByVal pArea As Short, ByVal pContents As ContentsFlag)
        area = pArea
        contents = pContents
    End Sub

    Public contents As ContentsFlag

    Public cluster As Short

    Public area As Short

    Public flags As Short

    Public mins As Short()

    Public maxs As Short()

    Public firstleafface As UShort

    Public numleaffaces As UShort

    Public firstleafbrush As UShort

    Public numleafbrushes As UShort

    Public leafWaterDataID As Short

End Structure

Public Structure Lump

    Public offset, length, version, fourCC As Integer

    Public type As LumpType

End Structure


Public Structure Node

    Public planenum As Integer

    Public children As Integer()

    Public mins As Short()

    Public maxs As Short()

    Public firstface As UShort

    Public numfaces As UShort

    Public area As Short

    Public paddding As Short

End Structure


Public Structure Plane

    Public normal As Vec3

    Public distance As Single

    Public type As Integer

End Structure
