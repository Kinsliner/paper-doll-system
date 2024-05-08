using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelAssetChecker : AssetChecker
{
    public override string Name => "�ҫ��ˬd";

    public override bool IsCheckable(Object asset)
    {
        if (asset == null)
        {
            return false;
        }
        if (IsAssetOf<ModelImporter>(asset))
        {
            return true;
        }
        return false;
    }

    public override CheckReport Check(Object asset)
    {
        ModelImporter modelImporter = GetAssetImporter(asset) as ModelImporter;
        if (modelImporter.IsNotNull())
        {
            bool check = true;
            List<(bool, string)> message = new List<(bool, string)>();
            CheckReadable(modelImporter, ref check, message);
            CheckImportBlendShapes(modelImporter, ref check, message);
            CheckImportVisibility(modelImporter, ref check, message);
            CheckImportCameras(modelImporter, ref check, message);
            CheckImportLights(modelImporter, ref check, message);

            List<string> reportMessage = ProcessMessage(message);

            if (check)
            {
                return new CheckReport(this, asset, true, "�ҫ��]�w���T");
            }
            else
            {
                return CheckReport.Fail(this, asset, reportMessage);
            }
        }

        return new CheckReport(this, asset, true, "����D�ҫ�");
    }

    private List<string> ProcessMessage(List<(bool, string)> message)
    {
        List<string> failMessage = new List<string>();
        foreach (var item in message)
        {
            if (item.Item1 == true)
            {
                ColorUtility.TryParseHtmlString(CheckToolEditor.PassColor, out Color color);

                failMessage.Add(item.Item2.ToColorString(color));
            }
            else if (item.Item1 == false)
            {
                ColorUtility.TryParseHtmlString(CheckToolEditor.FailColor, out Color color);

                failMessage.Add(item.Item2.ToColorString(color));
            }
        }
        return failMessage;
    }

    private void CheckReadable(ModelImporter modelImporter, ref bool check, List<(bool, string)> message)
    {
        if (modelImporter.isReadable == false)
        {
            check = false;
           
            message.Add((false, "�ҫ����]�w���iŪ�g"));
        }
        else
        {
            message.Add((true, "�ҫ��w�]�w���iŪ�g"));
        }
    }

    private void CheckImportBlendShapes(ModelImporter modelImporter, ref bool check, List<(bool, string)> message)
    {
        if (modelImporter.importBlendShapes != false)
        {
            check = false;
            message.Add((false, "�ҫ�������Import BlendShapes"));
        }
        else
        {
            
            message.Add((true, "�ҫ��w����Import BlendShapes"));
        }
    }

    private void CheckImportVisibility(ModelImporter modelImporter, ref bool check, List<(bool, string)> message)
    {
        if (modelImporter.importVisibility != false)
        {
            check = false;
            message.Add((false, "�ҫ�������Import Visibility"));
        }
        else
        {
            message.Add((true, "�ҫ��w����Import Visibility"));
        }
    }

    private void CheckImportCameras(ModelImporter modelImporter, ref bool check, List<(bool, string)> message)
    {
        if (modelImporter.importCameras != false)
        {
            check = false;
            message.Add((false, "�ҫ�������Import Cameras"));
        }
        else
        {
            message.Add((true, "�ҫ��w����Import Cameras"));
        }
    }

    private void CheckImportLights(ModelImporter modelImporter, ref bool check, List<(bool, string)> message)
    {
        if (modelImporter.importLights != false)
        {
            check = false;
            message.Add((false, "�ҫ�������Import Lights"));
        }
        else
        {
            message.Add((true, "�ҫ��w����Import Lights"));
        }
    }

    private bool foldout = false;
    public override void OnGUI()
    {
        base.OnGUI();

        using (new EditorGUI.IndentLevelScope(1))
        {
            foldout = EditorGUILayout.Foldout(foldout, "�ҫ��]�w");
            if (foldout)
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    GUIContent checkIcon = EditorGUIUtility.IconContent("Check");
                    GUIContent closeIcon = EditorGUIUtility.IconContent("Close");

                    checkIcon.text = "Read/Write".ToColorString(Color.white);
                    EditorGUILayout.LabelField(checkIcon, EditorExtension.RichTextLabel);
                    closeIcon.text = "Import BlendShapes".ToColorString(Color.white);
                    EditorGUILayout.LabelField(closeIcon, EditorExtension.RichTextLabel);
                    closeIcon.text = "Import Visibility".ToColorString(Color.white);
                    EditorGUILayout.LabelField(closeIcon, EditorExtension.RichTextLabel);
                    closeIcon.text = "Import Cameras".ToColorString(Color.white);
                    EditorGUILayout.LabelField(closeIcon, EditorExtension.RichTextLabel);
                    closeIcon.text = "Import Lights".ToColorString(Color.white);
                    EditorGUILayout.LabelField(closeIcon, EditorExtension.RichTextLabel);
                }
            }
        }
    }
}
