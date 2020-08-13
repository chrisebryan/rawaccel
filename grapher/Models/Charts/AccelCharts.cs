﻿using grapher.Models.Calculations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace grapher
{
    public class AccelCharts
    {
        public const int ChartSeparationVertical = 10;

        /// <summary> Needed to show full contents in form. Unsure why. </summary>
        public const int FormHeightPadding = 35;

        public AccelCharts(
            Form form,
            ChartXY sensitivityChart,
            ChartXY velocityChart,
            ChartXY gainChart,
            ToolStripMenuItem enableVelocityAndGain,
            ICollection<CheckBox> checkBoxesXY)
        {
            ContaingForm = form;
            SensitivityChart = sensitivityChart;
            VelocityChart = velocityChart;
            GainChart = gainChart;
            EnableVelocityAndGain = enableVelocityAndGain;
            CheckBoxesXY = checkBoxesXY;

            SensitivityChart.SetTop(0);
            VelocityChart.SetHeight(SensitivityChart.Height);
            VelocityChart.SetTop(SensitivityChart.Height + ChartSeparationVertical);
            GainChart.SetHeight(SensitivityChart.Height);
            GainChart.SetTop(VelocityChart.Top + VelocityChart.Height + ChartSeparationVertical);

            Rectangle screenRectangle = ContaingForm.RectangleToScreen(ContaingForm.ClientRectangle);
            FormBorderHeight = screenRectangle.Top - ContaingForm.Top;

            EnableVelocityAndGain.Click += new System.EventHandler(OnEnableClick);
            EnableVelocityAndGain.CheckedChanged += new System.EventHandler(OnEnableCheckStateChange);

            HideVelocityAndGain();
            ShowCombined();
        }

        public Form ContaingForm { get; }

        public ChartXY SensitivityChart { get; }

        public ChartXY VelocityChart { get; }

        public ChartXY GainChart { get; }

        public ToolStripMenuItem EnableVelocityAndGain { get; }

        private ICollection<CheckBox> CheckBoxesXY { get; }

        private bool Combined { get; set; }

        private int FormBorderHeight { get; }

        public void Bind(AccelData data)
        {
            if (Combined)
            {
                SensitivityChart.Bind(data.Combined.AccelPoints);
                VelocityChart.Bind(data.Combined.VelocityPoints);
                GainChart.Bind(data.Combined.GainPoints);
            }
            else
            {
                SensitivityChart.BindXY(data.X.AccelPoints, data.Y.AccelPoints);
                VelocityChart.BindXY(data.X.VelocityPoints, data.Y.VelocityPoints);
                GainChart.BindXY(data.X.GainPoints, data.Y.GainPoints);
            }
        }

        public void RefreshXY()
        {
            if (CheckBoxesXY.All(box => box.Checked))
            {
                ShowCombined();
            }
            else
            {
                ShowXandYSeparate();
            }
        }

        private void OnEnableClick(object sender, EventArgs e)
        {
            EnableVelocityAndGain.Checked = !EnableVelocityAndGain.Checked;
        }

        private void OnEnableCheckStateChange(object sender, EventArgs e)
        {
            if (EnableVelocityAndGain.Checked)
            {
                ShowVelocityAndGain();
            }
            else
            {
                HideVelocityAndGain();
            }
        }

        private void ShowVelocityAndGain()
        {
            VelocityChart.Show();
            GainChart.Show();
            ContaingForm.Height = SensitivityChart.Height + 
                                    ChartSeparationVertical +
                                    VelocityChart.Height +
                                    ChartSeparationVertical +
                                    GainChart.Height +
                                    FormBorderHeight;
        }

        private void HideVelocityAndGain()
        {
            VelocityChart.Hide();
            GainChart.Hide();
            ContaingForm.Height = SensitivityChart.Height + FormBorderHeight;
        }

        private void ShowXandYSeparate()
        {
            if (Combined)
            {
                SensitivityChart.SetSeparate();
                VelocityChart.SetSeparate();
                GainChart.SetSeparate();
                UpdateFormWidth();
                Combined = false;
            }
        }

        private void ShowCombined()
        {
            if (!Combined)
            {
                SensitivityChart.SetCombined();
                VelocityChart.SetCombined();
                GainChart.SetCombined();
                UpdateFormWidth();
                Combined = true;
            }
        }

        private void UpdateFormWidth()
        {
            ContaingForm.Width = SensitivityChart.Left + SensitivityChart.Width;
        }
    }
}
