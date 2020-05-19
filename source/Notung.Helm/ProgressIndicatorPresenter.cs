﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notung.Helm.Properties;
using Notung.Threading;
using System.Drawing;

namespace Notung.Helm
{
  public sealed class ProgressIndicatorPresenter
  {
    private readonly IProcessIndicatorView m_view;
    private readonly LaunchParameters m_launch_parameters;
    private readonly LengthyOperation m_operation;
    private CancellationTokenSource m_cancel_source;

    public ProgressIndicatorPresenter(LengthyOperation operation, LaunchParameters parameters, IProcessIndicatorView view)
    {
      if (operation == null)
        throw new ArgumentNullException("operation");

      if (parameters == null)
        throw new ArgumentNullException("parameters");

      if (view == null)
        throw new ArgumentNullException("view");

      m_launch_parameters = parameters;
      m_operation = operation;
      m_view = view;

      if (m_operation == null)
        throw new ArgumentException();
    }

    public void Initialize()
    {
      m_cancel_source = m_operation.GetCancellationTokenSource();

      if (m_cancel_source == null)
        m_view.ButtonVisible = false;

      m_launch_parameters.PropertyChanged += HanldePropertyChanged;
      m_launch_parameters.ShowCurrentSettings();

      m_operation.ProgressChanged += HandleProgressChanged;
      m_operation.ShowCurrentProgress();

      m_operation.TaskCompleted += HandleTaskCompleted;
    }

    private void HanldePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      m_view.Text = m_launch_parameters.Caption;
      m_view.Image = m_launch_parameters.Bitmap;
      m_view.SupportPercent = m_launch_parameters.SupportsPercentNotification;
      m_view.ButtonEnabled = m_launch_parameters.CanCancel;
    }

    private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (m_launch_parameters.SupportsPercentNotification)
        m_view.ProgressValue = e.ProgressPercentage;

      m_view.StateText = (e.UserState ?? string.Empty).ToString();
    }

    private void HandleTaskCompleted(object sender, EventArgs e)
    {
      m_launch_parameters.PropertyChanged -= HanldePropertyChanged;
      m_operation.ProgressChanged -= HandleProgressChanged;
      m_operation.TaskCompleted -= HandleTaskCompleted;

      m_view.ButtonVisible = true;
      m_view.ButtonEnabled = true;
      m_view.SupportPercent = true;

      if (m_cancel_source != null)
        m_cancel_source.Dispose();

      if (m_operation.Status != TaskStatus.RanToCompletion)
      {
        m_view.DialogResultOK = false;
        m_view.Close();
      }
      else if (m_launch_parameters.CloseOnFinish)
      {
        m_view.DialogResultOK = true;
        m_view.Close();
      }
      else
      {
        m_view.ButtonDialogResultOK = true;
        m_view.ButtonText = Resources.READY;
        m_view.ProgressValue = 100;
      }
    }

    public void ButtonClick()
    {
      if (m_cancel_source != null && m_operation.Status != TaskStatus.RanToCompletion)
        m_cancel_source.Cancel();
    }
  }

  public interface IProcessIndicatorView
  {
    bool ButtonVisible { get; set; }

    bool ButtonEnabled { get; set; }

    bool? ButtonDialogResultOK { get; set; }

    string ButtonText { get; set; }

    string Text { get; set; }

    Image Image { get; set; }

    bool SupportPercent { get; set; }

    int ProgressValue { get; set; }

    string StateText { get; set; }

    bool? DialogResultOK { get; set; }

    void Close();
  }
}
