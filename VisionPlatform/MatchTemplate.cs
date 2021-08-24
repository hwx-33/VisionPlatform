using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionDesigner;
using VisionDesigner.CircleFind;
using System.Diagnostics;
using MvdXmlParse;

namespace VisionPlatform
{
    public partial class MatchTemplate : Form
    {
        int TRUE_RESULT = 0;
        int FALSE_RESULT = -1;
        readonly Int32 _Scale = 5;        // 轮廓点放大倍数
        Stopwatch _StopWatch = null;     // 计时工具
        CCircleFindTool _CircleFindTool = null;     // 圆查找工具
        CMvdXmlParseTool _XmlParseTool = null;     // 运行参数解析工具
        CMvdImage _InputImage = null;     // 输入图像
        CMvdShape _ROI = null;     // ROI图形
        List<CMvdShape> _MaskShapeList = null;     // 屏蔽区图形列表
        CMvdAnnularSectorF _DrawArc = null;     // 绘制圆弧
        List<CMvdLineSegmentF> _DrawOutlineList = null;     // 绘制轮廓
        List<CMvdRectangleF> _DrawCaliperBoxList = null;     // 绘制卡尺框

        /// <summary>
        /// 清理圆弧
        /// </summary>
        /// <param name="drawArc"></param>
        private void ClearArc(ref CMvdAnnularSectorF drawArc)
        {
            if (null != drawArc)
            {
                mvdRenderActivex1.DeleteShape(drawArc);
                drawArc = null;
            }
        }
        /// <summary>
        /// 清理轮廓
        /// </summary>
        /// <param name="drawEdgePointList"></param>
        private void ClearOutline(List<CMvdLineSegmentF> drawEdgePointList)
        {
            foreach (var item in drawEdgePointList)
            {
                mvdRenderActivex1.DeleteShape(item);
            }
            drawEdgePointList.Clear();
        }

        /// <summary>
        /// 清理卡尺
        /// </summary>
        /// <param name="caliperBoxList"></param>
        private void ClearCaliperBoxList(List<CMvdRectangleF> caliperBoxList)
        {
            foreach (var item in caliperBoxList)
            {
                mvdRenderActivex1.DeleteShape(item);
            }
            caliperBoxList.Clear();
        }

        private void mvdRenderActivex1_MVDShapeChangedEvent(MVDRenderActivex.MVD_SHAPE_EVENT_TYPE enEventType, MVD_SHAPE_TYPE enShapeType, CMvdShape cShapeObj)
        {
            if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_SELECTED == enEventType)
            {
                return;
            }
            if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_EDITED == enEventType)
            {
                return;
            }

            if (MVD_SHAPE_TYPE.MvdShapePolygon == enShapeType)
            {

                if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_ADDED == enEventType)
                {
                    _MaskShapeList.Add(cShapeObj);
                }
                else if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_DELETED == enEventType)
                {
                    _MaskShapeList.Remove(cShapeObj);
                }
                return;
            }

            /* ROI or result shape may be delete */
            if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_DELETED == enEventType)
            {
                if (_ROI == cShapeObj)
                {
                    _ROI = null;
                }
                else if (_DrawArc == cShapeObj)
                {
                    _DrawArc = null;
                }
                else
                {
                    foreach (var item in _DrawOutlineList)
                    {
                        if (item == cShapeObj)
                        {
                            _DrawOutlineList.Remove(item);
                            break;
                        }
                    }

                    foreach (var item in _DrawCaliperBoxList)
                    {
                        if (item == cShapeObj)
                        {
                            _DrawCaliperBoxList.Remove(item);
                            break;
                        }
                    }
                }
                return;
            }


            MessageBox.Show("The shape that is inconsistent with the checked type will be deleted.\r\n");
            mvdRenderActivex1.DeleteShape(cShapeObj);
            mvdRenderActivex1.Display();
        }

        private void mvdRenderActivex1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 绘制圆弧
        /// </summary>
        /// <param name="drawArc"></param>
        private void DrawArc(CMvdAnnularSectorF drawArc)
        {
            drawArc.BorderColor = new MVD_COLOR(0, 255, 0, 255);
            mvdRenderActivex1.AddShape(drawArc);
        }

        /// <summary>
        /// 单个轮廓点十字放大
        /// </summary>
        /// <param name="edgePoint"></param>
        /// <param name="scale"></param>
        /// <param name="imageSize"></param>
        /// <param name="horizontalLine"></param>
        /// <param name="verticalLine"></param>
        private void ScaleEdgePoint(MVD_POINT_F edgePoint, int scale, out CMvdLineSegmentF horizontalLine, out CMvdLineSegmentF verticalLine)
        {
            horizontalLine = new CMvdLineSegmentF(new MVD_POINT_F(), new MVD_POINT_F());
            verticalLine = new CMvdLineSegmentF(new MVD_POINT_F(), new MVD_POINT_F());

            float minX = edgePoint.fX - scale;
            float maxX = edgePoint.fX + scale;
            float minY = edgePoint.fY - scale;
            float maxY = edgePoint.fY + scale;

            horizontalLine.StartPoint = new MVD_POINT_F(minX, edgePoint.fY);
            horizontalLine.EndPoint = new MVD_POINT_F(maxX, edgePoint.fY);
            verticalLine.StartPoint = new MVD_POINT_F(edgePoint.fX, minY);
            verticalLine.EndPoint = new MVD_POINT_F(edgePoint.fX, maxY);
        }
        /// <summary>
        /// 点是否在图像外
        /// </summary>
        /// <param name="point"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        private bool IsPointOutOfImage(MVD_POINT_F point, float imageWidth, float imageHeight)
        {
            if ((point.fX - 0 < Single.Epsilon)
            || (point.fY - 0 < Single.Epsilon)
            || (imageWidth - point.fX < Single.Epsilon)
            || (imageHeight - point.fY < Single.Epsilon))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 绘制轮廓
        /// </summary>
        /// <param name="drawEdgePointList"></param>
        private void DrawOutline(List<CCircleFindEdgePointInfo> drawEdgePointList, UInt32 imageWidth, UInt32 imageHeight)
        {
            foreach (var item in drawEdgePointList)
            {
                if (IsPointOutOfImage(item.EdgePoint, imageWidth, imageHeight))
                {
                    continue;
                }

                CMvdLineSegmentF horizontalLine = null;
                CMvdLineSegmentF verticalLine = null;
                ScaleEdgePoint(item.EdgePoint
                             , _Scale
                             , out horizontalLine
                             , out verticalLine);

                switch (item.Status)
                {
                    case MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_USED:
                        {
                            horizontalLine.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                            mvdRenderActivex1.AddShape(horizontalLine);
                            _DrawOutlineList.Add(horizontalLine);

                            verticalLine.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                            mvdRenderActivex1.AddShape(verticalLine);
                            _DrawOutlineList.Add(verticalLine);
                        }
                        break;
                    case MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_NO_USED:
                        {
                            horizontalLine.BorderColor = new MVD_COLOR(255, 255, 0, 255);
                            mvdRenderActivex1.AddShape(horizontalLine);
                            _DrawOutlineList.Add(horizontalLine);

                            verticalLine.BorderColor = new MVD_COLOR(255, 255, 0, 255);
                            mvdRenderActivex1.AddShape(verticalLine);
                            _DrawOutlineList.Add(verticalLine);
                        }
                        break;
                    case MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_NO_FIND:
                        {
                            horizontalLine.BorderColor = new MVD_COLOR(255, 0, 0, 255);
                            mvdRenderActivex1.AddShape(horizontalLine);
                            _DrawOutlineList.Add(horizontalLine);

                            verticalLine.BorderColor = new MVD_COLOR(255, 0, 0, 255);
                            mvdRenderActivex1.AddShape(verticalLine);
                            _DrawOutlineList.Add(verticalLine);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// .NET渲染控件鼠标事件
        /// </summary>
        /// <param name="enMouseEventType">鼠标事件类型</param>
        /// <param name="nPointX">窗口坐标X值</param>
        /// <param name="nPointY">窗口坐标Y值</param>
        /// <param name="nZDelta">标识鼠标滚轮方向</param>
        private void mvdRenderActivex1_MVDMouseEvent(MVDMouseEventType enMouseEventType, int nPointX, int nPointY, short nZDelta)
        {
            //用户想要实现自定义交互需通过SetConfiguration接口启用自定义交互
            //用户可根据enMouseEventType判断鼠标事件类型，编写对应的响应函数
            //示例：实时显示鼠标所在位置的图像坐标和像素值
            try
            {
                //窗口坐标转图像坐标
                float fImgX = 0.0f, fImgY = 0.0f;
                mvdRenderActivex1.TransformCoordinate(nPointX, nPointY, ref fImgX, ref fImgY, MVDCoordTransType.Wnd2Img);

                //获取像素信息显示
                do
                {
                    if (null == _InputImage)
                    {
                        break;
                    }

                    int nImagePointX = (int)fImgX;
                    int nImagePointY = (int)fImgY;
                    int nWidth = (int)_InputImage.Width;
                    int nHeight = (int)_InputImage.Height;
                    if (nImagePointX < 0 || nImagePointX >= nWidth
                        || nImagePointY < 0 || nImagePointY >= nHeight)
                    {
                        break;
                    }

                    string pixelInfo = string.Empty;
                    List<byte> pixelValue = _InputImage.GetPixel(nImagePointX, nImagePointY);
                    MVD_PIXEL_FORMAT enPixelFormat = _InputImage.PixelFormat;
                    if (MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 == enPixelFormat)
                    {
                        pixelInfo = string.Format("X:{0:D4} Y:{1:D4} | R:{2:D3} G:{3:D3} B:{4:D3}", nImagePointX, nImagePointY, pixelValue[0], pixelValue[0], pixelValue[0]);
                    }
                    else if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == enPixelFormat)
                    {
                        pixelInfo = string.Format("X:{0:D4} Y:{1:D4} | R:{2:D3} G:{3:D3} B:{4:D3}", nImagePointX, nImagePointY, pixelValue[0], pixelValue[1], pixelValue[2]);
                    }
                    else
                    {
                        throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, MVD_ERROR_CODE.MVD_E_SUPPORT, "Unsupported pixel format.");
                    }

                } while (false);
            }
            catch (MvdException ex)
            {
                
            }
            catch (System.Exception ex)
            {

            }
        }


        //直线查找
        public int HYFindLine(int PlatFormType, int CameraType)
        {
            return 0;
        }



        /// <summary>
        /// 绘制卡尺
        /// </summary>
        /// <param name="caliperBoxList"></param>
        private void DrawCaliperBoxList(List<CMvdRectangleF> caliperBoxList, UInt32 imageWidth, UInt32 imageHeight)
        {
            foreach (var item in caliperBoxList)
            {
                if (IsPointOutOfImage(new MVD_POINT_F(item.CenterX, item.CenterY), imageWidth, imageHeight))
                {
                    continue;
                }

                CMvdRectangleF drawCaliperBox = new CMvdRectangleF(item);
                drawCaliperBox.BorderColor = new MVD_COLOR(0, 0, 255, 255);
                mvdRenderActivex1.AddShape(drawCaliperBox);
                _DrawCaliperBoxList.Add(drawCaliperBox);
            }
        }

        //圆查找
        public int HYFindCircle()
        {
            

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"image(*.bmp;*.jpeg;*.jpg;*.png)|*.bmp;*.jpeg;*.jpg;*.png||";
                openFileDialog.RestoreDirectory = true;

                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    try
                    {
                        if (null == _InputImage)
                        {
                            _InputImage = new CMvdImage();
                        }
                        _InputImage.InitImage(openFileDialog.FileName);
                        if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == _InputImage.PixelFormat
                         || MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_P3 == _InputImage.PixelFormat)
                        {
                            _InputImage.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);

                        }

                        mvdRenderActivex1.LoadImageFromObject(_InputImage);
                        mvdRenderActivex1.Display();

                        _ROI = null;
                        _MaskShapeList.Clear();
                        _DrawArc = null;
                        _DrawOutlineList.Clear();
                        _DrawCaliperBoxList.Clear();


                    }
                    catch (MvdException ex)
                    {
                        return -1;
                    }
                    catch (System.Exception ex)
                    {
                        return -1;
                    }
                }
            }


            //try
            //{
            //    if ((null == _CircleFindTool) || (null == _InputImage))
            //    {
            //        throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, MVD_ERROR_CODE.MVD_E_CALLORDER);
            //    }

            //    // 清理绘制结果
            //    ClearArc(ref _DrawArc);
            //    ClearCaliperBoxList(_DrawCaliperBoxList);
            //    ClearOutline(_DrawOutlineList);

            //    // 算法处理
            //    _CircleFindTool.InputImage = _InputImage;
            //    _CircleFindTool.ROI = _ROI;

            //    _CircleFindTool.ClearMasks();
            //    foreach (var item in _MaskShapeList)
            //    {
            //        _CircleFindTool.AddMask(item);
            //    }

            //    _StopWatch.Restart();
            //    _CircleFindTool.Run();
            //    _StopWatch.Stop();

            //    CCircleFindResult circleFindResult = _CircleFindTool.Result;

            //    if (1 == circleFindResult.Status)
            //    {

            //        _DrawArc = new CMvdAnnularSectorF(circleFindResult.Arc);
            //        DrawArc(_DrawArc);
            //    }

            //    // 绘制轮廓点
            //    List<CCircleFindEdgePointInfo> circleFindEdgePointList = circleFindResult.EdgePointInfo;
            //    DrawOutline(circleFindEdgePointList, _InputImage.Width, _InputImage.Height);

            //    // 绘制卡尺
            //    DrawCaliperBoxList(circleFindResult.CaliperBoxList, _InputImage.Width, _InputImage.Height);
            //}
            //catch (MvdException ex)
            //{
            //    return FALSE_RESULT;
            //}
            //catch (System.Exception ex)
            //{
            //    return FALSE_RESULT;
            //}
            //finally
            //{
            //    mvdRenderActivex1.Display();
            //}
            return 0;
        }

        //模板匹配
        public int HYMatchTemplate(int PlatFormType, int CameraType)
        {
            return 0;
        }

        public MatchTemplate()
        {
            
            InitializeComponent();
            //HYFindCircle();
        }

        /// <summary>
        /// 更新DataGridView参数列表
        /// </summary>
        /// <param name="bufXml"></param>
        /// <param name="nXmlLen"></param>
        private void UpdateParamList(Byte[] bufXml, uint nXmlLen)
        {
            if (null == _XmlParseTool)
            {
                _XmlParseTool = new CMvdXmlParseTool(bufXml, nXmlLen);
            }
            else
            {
                _XmlParseTool.UpdateXmlBuf(bufXml, nXmlLen);
            }

            dataGridView1.Rows.Clear();
            for (int i = 0; i < _XmlParseTool.IntValueList.Count; ++i)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[dataGridView1.Columns["ParamNameBoxCol"].Index].Value = _XmlParseTool.IntValueList[i].Name;
                dataGridView1.Rows[index].Cells[dataGridView1.Columns["ParamValueBoxCol"].Index].Value = _XmlParseTool.IntValueList[i].CurValue;
            }

            for (int i = 0; i < _XmlParseTool.EnumValueList.Count; ++i)
            {
                int index = dataGridView1.Rows.Add();
                DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
                textBoxCell.Value = _XmlParseTool.EnumValueList[i].Name;
                dataGridView1.Rows[index].Cells[dataGridView1.Columns["ParamNameBoxCol"].Index] = textBoxCell;

                DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                for (int j = 0; j < _XmlParseTool.EnumValueList[i].EnumRange.Count; ++j)
                {
                    comboBoxCell.Items.Add(_XmlParseTool.EnumValueList[i].EnumRange[j].Name);
                }
                comboBoxCell.Value = _XmlParseTool.EnumValueList[i].CurValue.Name;
                dataGridView1.Rows[index].Cells[dataGridView1.Columns["ParamValueBoxCol"].Index] = comboBoxCell;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _CircleFindTool = new CCircleFindTool();
                byte[] runParamConfigBytes = new byte[256];
                uint runParamConfigDataSize = 256;
                uint runParamConfigDataLength = 0;

                try
                {
                    _CircleFindTool.SaveConfiguration(runParamConfigBytes, runParamConfigDataSize, ref runParamConfigDataLength);
                }
                catch (MvdException ex)
                {
                    if (MVD_ERROR_CODE.MVD_E_NOENOUGH_BUF == ex.ErrorCode)
                    {
                        runParamConfigBytes = new byte[runParamConfigDataLength];
                        runParamConfigDataSize = runParamConfigDataLength;
                        _CircleFindTool.SaveConfiguration(runParamConfigBytes, runParamConfigDataSize, ref runParamConfigDataLength);

                    }
                    else
                    {
                        throw ex;

                    }
                }
                UpdateParamList(runParamConfigBytes, runParamConfigDataLength);

            }
            catch (MvdException ex)
            {
                MessageBox.Show(ex.Message);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"image(*.bmp;*.jpeg;*.jpg;*.png)|*.bmp;*.jpeg;*.jpg;*.png||";
                openFileDialog.RestoreDirectory = true;

                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    try
                    {
                        if (null == _InputImage)
                        {
                            _InputImage = new CMvdImage();
                        }
                        _InputImage.InitImage(openFileDialog.FileName);
                        if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == _InputImage.PixelFormat
                         || MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_P3 == _InputImage.PixelFormat)
                        {
                            _InputImage.ConvertImagePixelFormat(MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                        }

                        mvdRenderActivex1.LoadImageFromObject(_InputImage);
                        mvdRenderActivex1.Display();

                        _ROI = null;
                        //_MaskShapeList.Clear();
                        _DrawArc = null;
                        //_DrawOutlineList.Clear();
                        //_DrawCaliperBoxList.Clear();


                    }
                    catch (MvdException ex)
                    {
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
