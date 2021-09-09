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
using VisionDesigner.LineFind;
using System.Diagnostics;
using MvdXmlParse;
using System.IO;

namespace VisionPlatform
{
    public partial class MatchTemplate : Form
    {
        readonly Int32 _Scale = 5;        // 轮廓点放大倍数
        private CLineFindTool m_stLineFindToolObj = null;
        private CMvdImage m_stInputImage = null;
        private CMvdShape m_stROIShape = null;
        List<VisionDesigner.CMvdShape> m_lMaskShapes = new List<VisionDesigner.CMvdShape>();
        private VisionDesigner.CMvdLineSegmentF m_stResLineShape = null;
        private CMvdXmlParseTool m_stXmlParseToolObj = null;
        List<CMvdLineSegmentF> _DrawOutlineList = new List<CMvdLineSegmentF>();     // 绘制轮廓
        List<CMvdRectangleF> _DrawCaliperBoxList = new List<CMvdRectangleF>();     // 绘制卡尺框

        public MatchTemplate()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Finish processing of the specific algorithm tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunTool_Click(object sender, EventArgs e)
        {
            try
            {
                if ((null == m_stLineFindToolObj) || (null == m_stInputImage))
                {
                    throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_TOOL, MVD_ERROR_CODE.MVD_E_CALLORDER);
                }

                // 清理绘制结果
                if (null != m_stResLineShape)
                {
                    mvdRenderActivex1.DeleteShape(m_stResLineShape);
                    m_stResLineShape = null;
                }
                ClearCaliperBoxList(_DrawCaliperBoxList);
                ClearOutline(_DrawOutlineList);

                m_stLineFindToolObj.InputImage = m_stInputImage;
                if (null == m_stROIShape)
                {
                    m_stLineFindToolObj.ROI = new VisionDesigner.CMvdRectangleF(m_stInputImage.Width / 2, m_stInputImage.Height / 2, m_stInputImage.Width, m_stInputImage.Height);
                }
                else
                {
                    m_stLineFindToolObj.ROI = m_stROIShape;
                }

                m_stLineFindToolObj.ClearMasks();
                foreach (var item in m_lMaskShapes)
                {
                    m_stLineFindToolObj.AddMask(item);
                }
                double fStartTime = GetTimeStamp();
                m_stLineFindToolObj.Run();
                double fCostTime = GetTimeStamp() - fStartTime;
                this.rtbInfoMessage.Text += "Running cost: " + fCostTime.ToString() + "\r\n";

                CLineFindResult stLineFindRes = m_stLineFindToolObj.Result;
                this.rtbInfoMessage.Text += "Recognition status: " + stLineFindRes.Status + "\r\n";
                if (1 == stLineFindRes.Status)
                {
                    this.rtbInfoMessage.Text += "Start point of line: (" + stLineFindRes.LineStartPoint.fX + ", " + stLineFindRes.LineStartPoint.fY + ")\r\n";
                    this.rtbInfoMessage.Text += "End point of line: (" + stLineFindRes.LineEndPoint.fX + ", " + stLineFindRes.LineEndPoint.fY + ")\r\n";
                    this.rtbInfoMessage.Text += "Angle: " + stLineFindRes.LineAngle + "\r\n";
                    this.rtbInfoMessage.Text += "Straightness: " + stLineFindRes.LineStraightness + "\r\n";
                    this.rtbInfoMessage.Text += "The number of edge point: " + stLineFindRes.EdgePointInfo.Count + "\r\n";

                    m_stResLineShape = new CMvdLineSegmentF(stLineFindRes.LineStartPoint, stLineFindRes.LineEndPoint);
                    m_stResLineShape.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                    mvdRenderActivex1.AddShape(m_stResLineShape);
                }

                // 绘制轮廓点
                List<CLineFindEdgePointInfo> lineFindEdgePointList = stLineFindRes.EdgePointInfo;
                DrawOutline(lineFindEdgePointList, m_stInputImage.Width, m_stInputImage.Height);

                // 绘制卡尺
                DrawCaliperBoxList(stLineFindRes.CaliperBoxList, m_stInputImage.Width, m_stInputImage.Height);
            }
            catch (MvdException ex)
            {
                this.rtbInfoMessage.Text += "An error occurred while running the tool. ErrorCode: 0x" + ex.ErrorCode.ToString("X") + "\r\n";
            }
            catch (System.Exception ex)
            {
                this.rtbInfoMessage.Text += "An error occurred while running the tool with ' " + ex.Message + " '\r\n";
            }
            finally
            {
                mvdRenderActivex1.Display();
            }
        }


        /// <summary>
        /// Set value of algorithm parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            String strName = dataGridView1.Rows[e.RowIndex].Cells[dataGridView1.Columns["ParamNameBoxCol"].Index].Value.ToString();
            string strCurParamVal = null;
            for (int i = 0; i < m_stXmlParseToolObj.IntValueList.Count; ++i)
            {
                if (strName == m_stXmlParseToolObj.IntValueList[i].Name)
                {
                    strCurParamVal = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
            }
            for (int i = 0; i < m_stXmlParseToolObj.EnumValueList.Count; ++i)
            {
                if (strName == m_stXmlParseToolObj.EnumValueList[i].Name)
                {
                    String strEnumEntryName = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    strCurParamVal = strEnumEntryName;
                }
            }
            try
            {
                double fStartTime = GetTimeStamp();
                m_stLineFindToolObj.SetRunParam(strName, strCurParamVal);
                double fCostTime = GetTimeStamp() - fStartTime;
                this.rtbInfoMessage.Text += "SetRunParam success. Cost: " + fCostTime.ToString() + "\r\n";
            }
            catch (MvdException ex)
            {
                this.rtbInfoMessage.Text += "Fail to set run param. ErrorCode: 0x" + ex.ErrorCode.ToString("X") + "\r\n";
            }
            catch (System.Exception ex)
            {
                this.rtbInfoMessage.Text += "Fail to set run param with error " + ex.Message + "\r\n";
            }
        }

        /// <summary>
        /// Response function of double-click event on information box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtbInfoMessage_DoubleClick(object sender, EventArgs e)
        {
            this.rtbInfoMessage.Clear();
        }

        /// <summary>
        /// Response function of ShapeChanged event from the MvRenderOCX
        /// </summary>
        /// <param name="enEventType"></param>
        /// <param name="enShapeType"></param>
        /// <param name="cShapeObj"></param>
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
                if ((MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_ADDED == enEventType) && (true != PolyMaskRadioButton.Checked))
                {
                    MessageBox.Show("Polygon is only allowed to be added only when adding masks.");
                    mvdRenderActivex1.DeleteShape(cShapeObj);
                    mvdRenderActivex1.Display();
                    return;
                }
                if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_ADDED == enEventType)
                {
                    m_lMaskShapes.Add(cShapeObj);
                }
                else if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_DELETED == enEventType)
                {
                    m_lMaskShapes.Remove(cShapeObj);
                }
                return;
            }

            /* ROI or result shape may be delete */
            if (MVDRenderActivex.MVD_SHAPE_EVENT_TYPE.MVD_SHAPE_DELETED == enEventType)
            {
                if (m_stROIShape == cShapeObj)
                {
                    m_stROIShape = null;
                }
                else if (m_stResLineShape == cShapeObj)
                {
                    m_stResLineShape = null;
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

            /* MvdShapeAdded event will be processed here */
            if (AllROIRadioButton.Checked)
            {
                MessageBox.Show("Shapes are not allowed to be added in the AllRegion Mode\r\n");
                mvdRenderActivex1.DeleteShape(cShapeObj);
                mvdRenderActivex1.Display();
                return;
            }
            if ((RectROIRadioButton.Checked) && (MVD_SHAPE_TYPE.MvdShapeRectangle == enShapeType))
            {
                if (null != m_stROIShape)
                {
                    mvdRenderActivex1.DeleteShape(m_stROIShape);
                    mvdRenderActivex1.Display();
                }
                m_stROIShape = cShapeObj;
                return;
            }

            MessageBox.Show("The shape that is inconsistent with the checked type will be deleted.\r\n");
            mvdRenderActivex1.DeleteShape(cShapeObj);
            mvdRenderActivex1.Display();
        }

        /// <summary>
        /// Update paramters
        /// </summary>
        /// <param name="bufXml"></param>
        /// <param name="nXmlLen"></param>
        private void UpdateParamList(Byte[] bufXml, uint nXmlLen)
        {
            if (null == m_stXmlParseToolObj)
            {
                m_stXmlParseToolObj = new CMvdXmlParseTool(bufXml, nXmlLen);
            }
            else
            {
                m_stXmlParseToolObj.UpdateXmlBuf(bufXml, nXmlLen);
            }
            dataGridView1.Rows.Clear();
            for (int i = 0; i < m_stXmlParseToolObj.IntValueList.Count; ++i)
            {
                int nIndex = dataGridView1.Rows.Add();
                dataGridView1.Rows[nIndex].Cells[dataGridView1.Columns["ParamNameBoxCol"].Index].Value = m_stXmlParseToolObj.IntValueList[i].Name;
                dataGridView1.Rows[nIndex].Cells[dataGridView1.Columns["ParamValueBoxCol"].Index].Value = m_stXmlParseToolObj.IntValueList[i].CurValue;
            }

            for (int i = 0; i < m_stXmlParseToolObj.EnumValueList.Count; ++i)
            {
                int nIndex = dataGridView1.Rows.Add();
                DataGridViewTextBoxCell textboxcell = new DataGridViewTextBoxCell();
                textboxcell.Value = m_stXmlParseToolObj.EnumValueList[i].Name;
                dataGridView1.Rows[nIndex].Cells[dataGridView1.Columns["ParamNameBoxCol"].Index] = textboxcell;
                DataGridViewComboBoxCell comboxcell = new DataGridViewComboBoxCell();
                for (int j = 0; j < m_stXmlParseToolObj.EnumValueList[i].EnumRange.Count; ++j)
                {
                    comboxcell.Items.Add(m_stXmlParseToolObj.EnumValueList[i].EnumRange[j].Name);
                }
                comboxcell.Value = m_stXmlParseToolObj.EnumValueList[i].CurValue.Name;
                dataGridView1.Rows[nIndex].Cells[dataGridView1.Columns["ParamValueBoxCol"].Index] = comboxcell;
            }
        }

        private double GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds;
        }

        private void RectROIRadioButton_Click(object sender, EventArgs e)
        {

        }

        private void AnnulROIRadioButton_Click(object sender, EventArgs e)
        {
            this.PolyMaskRadioButton.Checked = false;
        }

        private void PolyMaskRadioButton_Click(object sender, EventArgs e)
        {

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
        /// 绘制轮廓
        /// </summary>
        /// <param name="drawEdgePointList"></param>
        private void DrawOutline(List<CLineFindEdgePointInfo> drawEdgePointList, UInt32 imageWidth, UInt32 imageHeight)
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
                    case VisionDesigner.LineFind.MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_USED:
                        {
                            horizontalLine.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                            mvdRenderActivex1.AddShape(horizontalLine);
                            _DrawOutlineList.Add(horizontalLine);

                            verticalLine.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                            mvdRenderActivex1.AddShape(verticalLine);
                            _DrawOutlineList.Add(verticalLine);
                        }
                        break;
                    case VisionDesigner.LineFind.MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_NO_USED:
                        {
                            horizontalLine.BorderColor = new MVD_COLOR(255, 255, 0, 255);
                            mvdRenderActivex1.AddShape(horizontalLine);
                            _DrawOutlineList.Add(horizontalLine);

                            verticalLine.BorderColor = new MVD_COLOR(255, 255, 0, 255);
                            mvdRenderActivex1.AddShape(verticalLine);
                            _DrawOutlineList.Add(verticalLine);
                        }
                        break;
                    case VisionDesigner.LineFind.MVD_EDGEPOINT_STATUS.MVD_EDGEPOINT_STATUS_NO_FIND:
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                m_stLineFindToolObj = new CLineFindTool();
                byte[] fileBytes = new byte[256];
                uint nConfigDataSize = 256;
                uint nConfigDataLen = 0;
                try
                {
                    m_stLineFindToolObj.SaveConfiguration(fileBytes, nConfigDataSize, ref nConfigDataLen);
                }
                catch (MvdException ex)
                {
                    if (MVD_ERROR_CODE.MVD_E_NOENOUGH_BUF == ex.ErrorCode)
                    {
                        fileBytes = new byte[nConfigDataLen];
                        nConfigDataSize = nConfigDataLen;
                        m_stLineFindToolObj.SaveConfiguration(fileBytes, nConfigDataSize, ref nConfigDataLen);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                UpdateParamList(fileBytes, nConfigDataLen);

                this.rtbInfoMessage.Text += "Init finish.\r\n";
            }
            catch (MvdException ex)
            {
                this.rtbInfoMessage.Text += "Fail to initialize the tool. ErrorCode: 0x" + ex.ErrorCode.ToString("X") + "\r\n";
            }
            catch (System.Exception ex)
            {
                this.rtbInfoMessage.Text += "Fail with error " + ex.Message + "\r\n";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDlg = null;
            try
            {
                fileDlg = new System.Windows.Forms.OpenFileDialog();
                fileDlg.Filter = @"image(*.bmp;*.jpeg;*.jpg;*.png)|*.bmp;*.jpeg;*.jpg;*.png||";
                fileDlg.RestoreDirectory = true;

                if (fileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (null == m_stInputImage)
                    {
                        m_stInputImage = new CMvdImage();
                    }
                    m_stInputImage.InitImage(fileDlg.FileName);
                    mvdRenderActivex1.LoadImageFromObject(m_stInputImage);
                    mvdRenderActivex1.Display();

                    /* Shapes on the canvas are cleared by the render activeX when different images are switched */
                    m_lMaskShapes.Clear();
                    m_stROIShape = null;
                    m_stResLineShape = null;
                    _DrawOutlineList.Clear();
                    _DrawCaliperBoxList.Clear();
                    this.rtbInfoMessage.Text += "Finish loading image from [" + fileDlg.FileName + "].\r\n";
                }
                fileDlg.Dispose();
            }
            catch (MvdException ex)
            {
                this.rtbInfoMessage.Text += "Fail to load image from [" + fileDlg.FileName + "]. ErrorCode: 0x" + ex.ErrorCode.ToString("X") + "\r\n";
            }
            catch (System.Exception ex)
            {
                this.rtbInfoMessage.Text += "Fail to load image from [" + fileDlg.FileName + "]. Error: " + ex.Message + "\r\n";
            }
            finally
            {
                if (null != fileDlg)
                {
                    fileDlg.Dispose();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if ((null == m_stLineFindToolObj) || (null == m_stInputImage))
                {
                    throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_TOOL, MVD_ERROR_CODE.MVD_E_CALLORDER);
                }

                // 清理绘制结果
                if (null != m_stResLineShape)
                {
                    mvdRenderActivex1.DeleteShape(m_stResLineShape);
                    m_stResLineShape = null;
                }
                ClearCaliperBoxList(_DrawCaliperBoxList);
                ClearOutline(_DrawOutlineList);

                m_stLineFindToolObj.InputImage = m_stInputImage;
                if (null == m_stROIShape)
                {
                    m_stLineFindToolObj.ROI = new VisionDesigner.CMvdRectangleF(m_stInputImage.Width / 2, m_stInputImage.Height / 2, m_stInputImage.Width, m_stInputImage.Height);
                }
                else
                {
                    m_stLineFindToolObj.ROI = m_stROIShape;
                }

                m_stLineFindToolObj.ClearMasks();
                foreach (var item in m_lMaskShapes)
                {
                    m_stLineFindToolObj.AddMask(item);
                }
                double fStartTime = GetTimeStamp();
                m_stLineFindToolObj.Run();
                double fCostTime = GetTimeStamp() - fStartTime;
                this.rtbInfoMessage.Text += "Running cost: " + fCostTime.ToString() + "\r\n";

                CLineFindResult stLineFindRes = m_stLineFindToolObj.Result;
                this.rtbInfoMessage.Text += "Recognition status: " + stLineFindRes.Status + "\r\n";
                if (1 == stLineFindRes.Status)
                {
                    this.rtbInfoMessage.Text += "Start point of line: (" + stLineFindRes.LineStartPoint.fX + ", " + stLineFindRes.LineStartPoint.fY + ")\r\n";
                    this.rtbInfoMessage.Text += "End point of line: (" + stLineFindRes.LineEndPoint.fX + ", " + stLineFindRes.LineEndPoint.fY + ")\r\n";
                    this.rtbInfoMessage.Text += "Angle: " + stLineFindRes.LineAngle + "\r\n";
                    this.rtbInfoMessage.Text += "Straightness: " + stLineFindRes.LineStraightness + "\r\n";
                    this.rtbInfoMessage.Text += "The number of edge point: " + stLineFindRes.EdgePointInfo.Count + "\r\n";

                    m_stResLineShape = new CMvdLineSegmentF(stLineFindRes.LineStartPoint, stLineFindRes.LineEndPoint);
                    m_stResLineShape.BorderColor = new MVD_COLOR(0, 255, 0, 255);
                    mvdRenderActivex1.AddShape(m_stResLineShape);
                }

                // 绘制轮廓点
                List<CLineFindEdgePointInfo> lineFindEdgePointList = stLineFindRes.EdgePointInfo;
                DrawOutline(lineFindEdgePointList, m_stInputImage.Width, m_stInputImage.Height);

                // 绘制卡尺
                DrawCaliperBoxList(stLineFindRes.CaliperBoxList, m_stInputImage.Width, m_stInputImage.Height);
            }
            catch (MvdException ex)
            {
                this.rtbInfoMessage.Text += "An error occurred while running the tool. ErrorCode: 0x" + ex.ErrorCode.ToString("X") + "\r\n";
            }
            catch (System.Exception ex)
            {
                this.rtbInfoMessage.Text += "An error occurred while running the tool with ' " + ex.Message + " '\r\n";
            }
            finally
            {
                mvdRenderActivex1.Display();
            }
        }

    }
}
