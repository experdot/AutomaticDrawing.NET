﻿Public Class ImageProcessClass
    ''' <summary> 
    ''' 根据指定阈值判断两个颜色是否相近
    ''' </summary> 
    Public Function CompareRGB(ByVal Color1 As Color, ByVal Color2 As Color, ByVal Distance As Byte) As Boolean
        Dim r As Integer = Int(Color1.R) - Int(Color2.R)
        Dim g As Integer = Int(Color1.G) - Int(Color2.G)
        Dim b As Integer = Int(Color1.B) - Int(Color2.B)
        Dim absDis As Integer = Math.Sqrt(r * r + g * g + b * b)
        If absDis < Distance Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 返回指定颜色的中值
    ''' </summary> 
    Public Function gethHD(ByVal color1 As Color)
        Dim HD, r, g, b As Integer
        r = color1.R
        g = color1.G
        b = color1.B
        HD = (r + g + b) / 3
        Return HD
    End Function
    ''' <summary>
    ''' 返回指定区域的屏幕图像
    ''' </summary>
    ''' <param name="gX"></param>
    ''' <param name="gY"></param>
    ''' <param name="gWidth"></param>
    ''' <param name="gHeight"></param>
    ''' <returns></returns>
    Public Function GetScreenImage(ByVal gX As Integer, ByVal gY As Integer, ByVal gWidth As Integer, ByVal gHeight As Integer) As Bitmap
        Dim ResultBitmap As New Bitmap(gWidth, gHeight)
        Using pg As Graphics = Graphics.FromImage(ResultBitmap)
            pg.CopyFromScreen(gX, gY, 0, 0, New Size(gWidth, gHeight))
        End Using
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定文字生成的图像
    ''' </summary>
    ''' <param name="gString"></param>
    ''' <param name="gFont"></param>
    ''' <param name="gWidth"></param>
    ''' <param name="gHeight"></param>
    ''' <returns></returns>
    Public Function GetTextImage(ByVal gString As String, ByVal gFont As Font, ByVal gWidth As Integer, ByVal gHeight As Integer) As Bitmap
        Dim ResultBitmap As New Bitmap(gWidth, gHeight)
        Using pg = Graphics.FromImage(ResultBitmap)
            pg.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias '抗锯齿
            pg.DrawString(gString, gFont, Brushes.Black, 0, 0)
        End Using
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定图像的二值化图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <param name="gSplitNum"></param>
    ''' <returns></returns>
    Public Function GetThresholdImage(ByVal gBitmap As Bitmap, ByVal gSplitNum As Byte) As Bitmap
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim TempHD As Integer
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                TempHD = gethHD(gBitmap.GetPixel(i, j))
                ResultBitmap.SetPixel(i, j, IIf(TempHD < gSplitNum, Color.Black, Color.White))
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定图像的轮廓图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <param name="gDistance"></param>
    ''' <returns></returns>
    Public Function GetOutLineImage(ByVal gBitmap As Bitmap, ByVal gDistance As Byte) As Bitmap
        Dim xArray2() As Short = {0, 1, 0, -1}
        Dim yArray2() As Short = {-1, 0, 1, 0}
        'Dim ResultBitmap As New Bitmap(gBitmap) '在原图的基础上绘图
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim Color1, Color2 As Color
        For i = 1 To gBitmap.Width - 2
            For j = 1 To gBitmap.Height - 2
                For p = 0 To 3
                    Color1 = gBitmap.GetPixel(i, j)
                    Color2 = gBitmap.GetPixel(i + xArray2(p), j + yArray2(p))
                    If CompareRGB(Color1, Color2, gDistance) = False And gethHD(Color1) - gethHD(Color2) > 0 Then
                        ResultBitmap.SetPixel(i, j, Color.Black)
                    Else
                        ResultBitmap.SetPixel(i, j, Color.White)
                    End If
                Next
            Next
        Next
        Return ResultBitmap
    End Function

    ''' <summary>
    ''' 返回指定图像的空心图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetAroundImage(gBitmap As Bitmap)
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ImageBolArr(,) As Integer = GetImageBol(gBitmap)
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If ImageBolArr(i, j) = 1 AndAlso CheckPointAround(ImageBolArr, i, j) = False Then
                    ResultBitmap.SetPixel(i, j, Color.Black)
                Else
                    ResultBitmap.SetPixel(i, j, Color.White)
                End If
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定图像的反相图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetInvertImage(gBitmap As Bitmap)
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ImageBolArr(,) As Integer = GetImageBol(gBitmap)
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If ImageBolArr(i, j) = 1 Then
                    ResultBitmap.SetPixel(i, j, Color.White)
                Else
                    ResultBitmap.SetPixel(i, j, Color.Black)
                End If
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定图像的二值化数据
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Private Function GetImageBol(ByVal gBitmap As Bitmap) As Integer(,)
        Dim ResultArr(gBitmap.Width - 1, gBitmap.Height - 1) As Integer
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If gBitmap.GetPixel(i, j).Equals(Color.FromArgb(0, 0, 0)) = True Then
                    ResultArr(i, j) = 1
                Else
                    ResultArr(i, j) = 0
                End If
            Next
        Next
        Return ResultArr
    End Function
    ''' <summary>
    ''' 检查一个点是否被包围
    ''' </summary>
    ''' <param name="BolArr"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    Private Function CheckPointAround(BolArr As Integer(,), ByVal x As Integer, ByVal y As Integer) As Boolean
        If Not (x > 0 And y > 0 And x < BolArr.GetUpperBound(0) And y < BolArr.GetUpperBound(1)) Then Return True
        If BolArr(x - 1, y) = 1 And BolArr(x + 1, y) = 1 And BolArr(x, y - 1) = 1 And BolArr(x, y + 1) = 1 Then
            Return True '当前点为实体内部
        Else
            Return False '当前点为实体边缘
        End If
    End Function
End Class
