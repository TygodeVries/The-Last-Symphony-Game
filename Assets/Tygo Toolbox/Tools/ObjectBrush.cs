
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;
using Event = UnityEngine.Event;

public class ObjectBrushWindow : EditorWindow
{
    [MenuItem("Tygo/Object Brush")]
    public static void ShowExample()
    {
        ObjectBrushWindow wnd = GetWindow<ObjectBrushWindow>();
        wnd.titleContent = new GUIContent("Object Brush");

    }

    public static GameObject prefab;
    public static bool active;
    public static Vector3 offset;
    public static float minimumDistance = 1;

    public static Vector3 randomOffset = new Vector3(0, 0, 0);
    public static Vector3 randomRotation = new Vector3(0, 0, 0);
    public static float randomScale = 0;

    public void OnGUI()
    {
        GUIStyle bold = new GUIStyle();
        bold.fontStyle = FontStyle.Bold;
        bold.normal.textColor = Color.white;

        GUILayout.Space(20);
        GUILayout.Label("Object Brush", bold);
        active = GUILayout.Toggle(active, "Enabled");

        GUILayout.Space(20);
        GUILayout.Label("Brush Settings", bold);

        minimumDistance = EditorGUILayout.FloatField("Minimum Distance", minimumDistance);

        GUILayout.Space(20);
        GUILayout.Label("Object Settings", bold);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Prefab");
        prefab = (GameObject) EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
        GUILayout.EndHorizontal();

        offset = EditorGUILayout.Vector3Field("Offset", offset);

        GUILayout.Space(20);
        GUILayout.Label("Randomness", bold);
        GUILayout.Label("Scale gets added to the base size of a component.");
        GUILayout.Label("Using the value below, a random point will be picked withing the negative and positive of the value");
        GUILayout.Label("This will be added to the base value.");
        GUILayout.Label("Example:");
        GUILayout.Label("Random Scale is 0.1, A random value between [-0.1 and 0.1] gets picked, and added to the base scale.");
        GUILayout.Space(20);

        randomOffset = EditorGUILayout.Vector3Field("Random Offset", randomOffset);

        randomRotation = EditorGUILayout.Vector3Field("Random Rotation", randomRotation);

        randomScale = EditorGUILayout.FloatField("Random Scale", randomScale);
    }
}

[InitializeOnLoad]
public class ObjectBrushEditor
{
    static ObjectBrushEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static Vector3 lastPlacePoint;

    private static void OnSceneGUI(SceneView sceneView)
    {
        if(ObjectBrushWindow.active)
        {
            // Only respond to mouse down event
            UnityEngine.Event e = UnityEngine.Event.current;

            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            bool hit = Physics.Raycast(worldRay, out RaycastHit hitInfo);



            // Update cursor
            if (hit)
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0,50000, 50000), MouseCursor.ArrowPlus);
            }
            else
            {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, 50000, 50000), MouseCursor.Arrow);
            }

            if (e.type == EventType.MouseDrag && e.button == 0)
            {
                e.Use();

                if (Vector3.Distance(hitInfo.point, lastPlacePoint) > ObjectBrushWindow.minimumDistance)
                {
                    GameObject gm = GameObject.Instantiate(ObjectBrushWindow.prefab);
                    gm.transform.position = hitInfo.point + ObjectBrushWindow.offset;

                    lastPlacePoint = hitInfo.point;

                    Undo.RegisterCreatedObjectUndo(gm, "Brushed GameObject");

                    gm.transform.position += randomRange(ObjectBrushWindow.randomOffset);
                    gm.transform.Rotate(randomRange(ObjectBrushWindow.randomRotation));

                    float scale = Random.Range(-ObjectBrushWindow.randomScale, ObjectBrushWindow.randomScale);
                    gm.transform.localScale += new Vector3(scale, scale, scale);
                }
            }
        }
    }

    public static Vector3 randomRange(Vector3 a)
    {
        float x = Random.Range(-a.x, a.x);
        float y = Random.Range(-a.y, a.y);
        float z = Random.Range(-a.z, a.z);

        return new Vector3(x, y, z);
    }
}
#endif