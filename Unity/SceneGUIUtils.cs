using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BB
{
#if UNITY_EDITOR
    public static class SceneGUIUtils
    {
        const float LineThickness = 3;
        //public static void ExecuteDuringSceneOnGUI(Action action, string undoName = "")
        //{
        //	Action<SceneView> newAction = null;
        //	SceneView.duringSceneGui += newAction = (view) =>
        //	{
        //		if (undoName.NoE())
        //			action();
        //		else
        //			EditorUtils.GroupUndos(action, undoName);
        //		SceneView.duringSceneGui -= newAction;
        //	};
        //}
        public static void Repaint()
        {
            SceneView.RepaintAll();
        }
        public static void RepaintGameView()
        {
            System.Reflection.Assembly assembly = typeof(EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            EditorWindow gameview = EditorWindow.GetWindow(type);
            gameview.Repaint();
        }
        public static void RepaintAllViews()
        {
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        public static Vector2 GetOrthogonalMousePos()
        {
            var origin = GetMouseOrigin();
            var worldPos = new Vector3(origin.x, origin.y, SceneView.currentDrawingSceneView.camera.nearClipPlane);
            return worldPos;
        }
        public static Vector3 GetMouseOrigin()
        {
            return GetMouseRay().origin;
        }
        public static Ray GetMouseRay()
        {
            Event e = Event.current;
            var view = SceneView.currentDrawingSceneView;
            if (!view)
                view = SceneView.lastActiveSceneView;
            var camera = view.camera;
            Handles.SetCamera(camera);
            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            return ray;
        }
        public static Vector3 GetMouseHit(int mask)
        {
            var ray = GetMouseRay();
            Vector3 result;
            if (Physics.Raycast(ray, out var hitInfo, 1000, mask, QueryTriggerInteraction.Ignore))
            {
                result = hitInfo.point;
            }
            else
            {
                result = GetHitOnPlane(ray, ray.direction, Vector3.zero);
            }
            return result;
        }
        public static GameObject GetMouseHitObject()
        {
            GameObject result = null;
            var ray = GetMouseRay();
            if (Physics.Raycast(ray, out var hitInfo, 1000, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                result = hitInfo.collider.gameObject;
            }
            return result;
        }
        public static Vector3 GetMouseHitOnPlane(Vector3 normal, Vector3 position)
        {
            return GetHitOnPlane(GetMouseRay(), normal, position);
        }
        private static Vector3 GetHitOnPlane(Ray ray, Vector3 normal, Vector3 position)
        {
            var result = ray.origin;
            if (new Plane(normal, position).Raycast(ray, out var enter))
            {
                result += ray.direction * enter;
            }
            return result;
        }
        public static void BlockFocus()
        {
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            Event.current?.Use();
        }
        //public static bool IsInSelectionHierarchy(this GameObject obj)
        //{
        //	var result = Selection.gameObjects.AnyTrue((selected) => selected.IsInSameHierarchy(obj));
        //	return result;
        //}
        //public static void SetColor(Color color)
        //{
        //	Handles.color = color;
        //}
        //public static void DrawLines(Color color, float z, params Vector2[] points)
        //{
        //	var points3D = points.Convert((i, v) => new Vector3(v.x, v.y, z));
        //	DrawLines(color, points3D);
        //}

        public static void DrawLines(params Vector3[] points)
        {
            for (var i = 0; i < points.Length - 1; i++)
                Handles.DrawLine(points[i], points[i + 1], LineThickness);
        }
       
        public static void DrawDottedLine(Color color, Vector3 from, Vector3 to)
        {
            ApplyColor(color, () => Handles.DrawDottedLine(from, to, 3));
        }
        public static void DrawRect(Color faceColor, Color outlineColor, Vector3 center, Vector3 planeRight, Vector3 planeUp, Rect bounds)
        {
            var points = GetRectangleCorners(center, planeRight, planeUp, bounds);
            Handles.DrawSolidRectangleWithOutline(points, faceColor, outlineColor);
        }
        private static Vector3[] GetRectangleCorners(Vector3 center, Vector3 right, Vector3 up, Rect bounds)
        {
            var xMin = bounds.xMin;
            var yMin = bounds.yMin;
            var xMax = bounds.xMax;
            var yMax = bounds.yMax;
            var result = new Vector3[]
            {
            Point(xMin, yMin),
            Point(xMin, yMax),
            Point(xMax, yMax),
            Point(xMax, yMin)
            };
            return result;
            Vector3 Point(float x, float y)
            {
                return center + right * x + up * y;
            }
        }
        public static void DrawFlatCone(Color color, Vector3 center, Vector3 dir, Vector3 up, float angle, float radius)
        {
            if (angle > 0f)
            {
                dir.Normalize();
                var beginDir = Quaternion.AngleAxis(-angle * 0.5f, up) * dir;
                var endDir = Quaternion.AngleAxis(angle * 0.5f, up) * dir;
                DrawWireArc(center, beginDir, up, angle, radius);
                DrawLines(center + beginDir * radius, center, center + endDir * radius);
            }
        }
        public static void DrawWireArc(Vector3 center, Vector3 beginDir, Vector3 up, float angle, float radius, float thickness = 0)
        {
            Handles.DrawWireArc(center, up, beginDir, angle, radius);
        }
        public static void DrawDroplet(Color color, Vector3 center, Vector3 forward, Vector3 up, float radius, float pointDistance, float arcAngle)
        {
            var angle = (360 - arcAngle).Min0() * 0.5f;
            var beginDir = Quaternion.AngleAxis(angle, up) * forward;
            DrawWireArc(center, beginDir, up, arcAngle, radius);
            var endDir = Quaternion.AngleAxis(-angle, up) * forward;
            DrawLines(beginDir * radius + center, forward * pointDistance + center, endDir * radius + center);
        }
        private static void DrawFlatArrowHead(Color color, Vector3 position, Quaternion rotation, float size, float angle)
        {
            var right = Mathf.Cos(angle) * size * (rotation * Vector3.right);
            var back = Mathf.Sin(angle) * size * (rotation * Vector3.back);
            DrawLines(position - right + back, position, position + right + back);
        }
        public static void DrawFlatArrowHead(Color color, Vector3 position, Vector3 direction, float size, float angle)
        {
            var dir = direction.normalized * size;
            angle *= Mathf.Deg2Rad;
            var right = Vector3.Cross(dir, Vector3.up) * Mathf.Sin(angle);
            var back = position - dir * Mathf.Cos(angle);
            var rotation = Quaternion.FromToRotation(Vector3.forward, direction);
            DrawLines(back - right, position, back + right);
        }
        public static void DrawFlatArrow(Color color, Vector3 from, Vector3 to, Direction dir, float size = 0.1f, float angle = 30)
        {
            DrawLines(from, to);
            if (dir.HasFlag(Direction.Forward))
                DrawFlatArrowHead(color, to, to - from, size, angle);
            if (dir.HasFlag(Direction.Back))
                DrawFlatArrowHead(color, from, from - to, size, angle);
        }
        public static void DrawFlatArrow(Color color, Transform t, Direction dir, float length = 1f)
        {
            if (t)
                DrawFlatArrow(color, t.position, t.position + t.forward * length, dir);
        }
        //public static Vector3[] GetRectCorners(Rect3D rect, out Vector3 right, out Vector3 up)
        //{
        //	right = rect.Rotation * Vector3.right;
        //	up = rect.Rotation * Vector3.up;
        //	var hb = rect.Size * 0.5f;
        //	var corners = new Vector3[]
        //		{
        //		rect.Position-right*hb.x-up*hb.y,
        //		rect.Position-right*hb.x+up*hb.y,
        //		rect.Position+right*hb.x+up*hb.y,
        //		rect.Position+right*hb.x-up*hb.y
        //		};
        //	return corners;
        //}
        //public static void DrawRect(Color faceColor, Color outlineColor, Rect3D rect)
        //{
        //	var corners = GetRectCorners(rect, out _, out _);
        //	Handles.DrawSolidRectangleWithOutline(corners, faceColor, outlineColor);
        //}
        //public static bool DrawEditablePoly(Color color, Poly3D poly, bool editCorners, bool editEdges, bool quad, out Poly3D newPoly)
        //{
        //	var result = false;
        //	var right = poly.Rotation * Vector3.right;
        //	var up = poly.Rotation * Vector3.up;
        //	var p = new Poly3D
        //	{
        //		Position = poly.Position,
        //		Rotation = poly.Rotation,
        //		Corners = poly.Corners.Clone() as Vector2[]
        //	};
        //	draw edges

        //	if (!quad)
        //	{
        //		for (int i = 0; i < poly.Corners.Length - 1; i++)
        //		{
        //			DrawEdge(i, i + 1, true, true);
        //		}
        //		DrawEdge(poly.Corners.Length - 1, 0, true, true);
        //	}
        //	else
        //	{
        //		DrawEdge(0, 1, false, true);
        //		DrawEdge(1, 2, true, false);
        //		DrawEdge(2, 3, false, true);
        //		DrawEdge(3, 0, true, false);
        //	}
        //	for (int i = 0; i < poly.Corners.Length; i++)
        //	{
        //		if (editCorners)
        //			EditPoint(GetPosition(poly.Corners[i]), true, true, i);
        //	}
        //	void DrawEdge(int iFrom, int iTo, bool editX, bool editY)
        //	{
        //		var from = GetPosition(poly.Corners[iFrom]);
        //		var to = GetPosition(poly.Corners[iTo]);
        //		DrawLines(color, from, to);
        //		edit edges

        //		if (editEdges)
        //			EditPoint((from + to) * 0.5f, editX, editY, iFrom, iTo);
        //	}
        //	void EditPoint(Vector3 position, bool editX, bool editY, params int[] editedCorners)
        //	{
        //		if (EditPosition(color, position, Quaternion.identity, 0.05f, false, Handles.DotHandleCap, out var newPos))
        //		{
        //			var diff3d = newPos - position;
        //			var diff = new Vector2(Vector3.Dot(diff3d, right), Vector3.Dot(diff3d, up));
        //			if (!editX)
        //				diff.x = 0;
        //			if (!editY)
        //				diff.y = 0;
        //			foreach (var iCorner in editedCorners)
        //				p.Corners[iCorner] += diff;
        //			result = true;
        //		}
        //	}
        //	Vector3 GetPosition(Vector2 corner) => poly.Position + right * corner.x + up * corner.y;
        //	edit corners

        //	newPoly = p;
        //	return result;
        //}
        //public static bool DrawEditableRect(Color faceColor, Color outlineColor, Color cornerColor, Rect3D rect, bool editCorners, bool editEdges, out Rect3D newRect)
        //{
        //	var result = false;
        //	var corners = GetRectCorners(rect, out var right, out var up);
        //	Handles.DrawSolidRectangleWithOutline(corners, faceColor, outlineColor);
        //	if (editCorners)
        //	{
        //		DrawCorner(corners[0], xMin, yMin);
        //		DrawCorner(corners[1], xMin, yMax);
        //		DrawCorner(corners[2], xMax, yMax);
        //		DrawCorner(corners[3], xMax, yMin);
        //	}
        //	if (editEdges)
        //	{
        //		DrawEdge(corners[0], corners[1], xMin, null);
        //		DrawEdge(corners[1], corners[2], null, yMax);
        //		DrawEdge(corners[2], corners[3], xMax, null);
        //		DrawEdge(corners[3], corners[0], null, yMin);
        //	}
        //	newRect = rect;
        //	return result;
        //	void xMin(float value) => rect.Size.x -= value;
        //	void xMax(float value) => rect.Size.x += value;
        //	void yMin(float value) => rect.Size.y -= value;
        //	void yMax(float value) => rect.Size.y += value;
        //	void DrawEdge(Vector3 corner1, Vector3 corner2, Action<float> processMoveX, Action<float> processMoveY)
        //	{
        //		var point = (corner1 + corner2) * 0.5f;
        //		DrawCorner(point, processMoveX, processMoveY);
        //	}
        //	void DrawCorner(Vector3 point, Action<float> processMoveX, Action<float> processMoveY)
        //	{
        //		if (EditPosition(cornerColor, point, Quaternion.identity, 0.05f, false, Handles.DotHandleCap, out var newPos))
        //		{
        //			var diff = newPos - point;
        //			if (processMoveX != null)
        //			{
        //				var x = Vector3.Dot(diff, right);
        //				processMoveX.Invoke(x);
        //				rect.Position += right * x * 0.5f;
        //			}
        //			if (processMoveY != null)
        //			{
        //				var y = Vector3.Dot(diff, up);
        //				processMoveY.Invoke(y);
        //				rect.Position += up * y * 0.5f;
        //			}
        //			result = true;
        //		}
        //	}
        //}
        public static void DrawAABB(Color color, Vector2 p1, Vector2 p2)
        {
            var points = new Vector3[]
            {
                p1,
                new Vector2(p1.x,p2.y),
                p2,
                new Vector2(p2.x,p1.y)
            };
            DrawBox(Color.clear, color, points);
        }
        public static IDisposable ApplyColor(Color color)
        {
            Color originalColor = Handles.color;
            Handles.color = color;
            return ActionOnDispose.GetPooled(() => Handles.color = originalColor);
        }
        public static void DrawWorldPoint(Color color, Vector3 position, float size, Action onClick)
        {
            Vector2 screenPos = HandleUtility.WorldToGUIPoint(position);

            Color originalColor = Handles.color;

            // Draw button background
            Handles.color = color;
            if (DrawClickablePoint(position, size, true))
                //Handles.Button(
                //position, 
                //Quaternion.identity, 
                //size * 0.2f, 
                //size * 0.2f, 
                //Handles.SphereHandleCap))
                onClick();
            Handles.color = originalColor;
        }
        private static T ApplyColor<T>(Color color, Func<T> func)
        {
            var c = Handles.color;
            Handles.color = color;
            var result = func();
            Handles.color = c;
            return result;
        }
        private static void ApplyColor(Color color, Action a)
        {
            var c = Handles.color;
            Handles.color = color;
            a();
            Handles.color = c;
        }
        private static void DrawHandle(Color color, Vector3 pos, Quaternion rot, float size, bool constantSize, Handles.CapFunction method)
            => ApplyColor(color, () => method(0, pos, rot, GetSize(pos, size, constantSize), EventType.Repaint));
        public enum TransformType
        {
            Translate = 1,
            Rotate = 2,
            Scale = 4
        }
        //public static void PointEditTransform(Transform transform, Color selectedColor, float size, bool showGizmo, Action<Vector3> translate)
        //{
        //	EditTransform(transform, selectedColor, size, Handles.DotHandleCap, showGizmo, translate);
        //}
        //public static void DrawMovementTool(Vector3 position, Quaternion orientation, Action<Vector3> translate)//, Action<Quaternion> rotate, Action<Vector3> scale)
        //{
        //	EditTool(position, orientation, Color.white, 0, null, true, translate);
        //	switch (Tools.current)
        //	{
        //		case Tool.Move:
        //			Transform(() => Handles.PositionHandle(position, orientation), translate);
        //			break;
        //			//case Tool.Rotate:
        //			//    Transform(Handles.RotationHandle, rotate);
        //			//    break;
        //			//case Tool.Scale:
        //			//    Transform(Handles.sca)
        //	}
        //	void Transform<T>(Func<T> getter, Action<T> setter)
        //	{
        //		EditorGUI.BeginChangeCheck();
        //		var t = getter();// Handles.PositionHandle(position, orientation);
        //		if (EditorGUI.EndChangeCheck())
        //		{
        //			setter(t);
        //			// translate(pos);
        //		}
        //	}
        //	//DrawHandlesButton(color, transform.position, size, capMethod);
        //	if (Tools.current == Tool.Transform)
        //	{
        //		EditorGUI.BeginChangeCheck();
        //		var pos = Handles.PositionHandle(position, orientation);
        //		if (EditorGUI.EndChangeCheck())
        //		{
        //			translate(pos);
        //		}
        //	}
        //}
        //public static void EditTransform(Transform t, Action<Vector3> translate, Action<Quaternion> rotate)
        //{
        //	EditTool(t.position, Quaternion.identity, translate, rotate);
        //}
        //public static void EditTool(Vector3 position, Quaternion orientation, Action<Vector3> translate, Action<Quaternion> rotate)
        //{
        //	var tool = Tools.current;
        //	switch (tool)
        //	{
        //		case Tool.Move:
        //			ProcessChange(() => Handles.PositionHandle(position, orientation), translate);
        //			break;
        //		case Tool.Rotate:
        //			ProcessChange(() => Handles.RotationHandle(orientation, position), rotate);
        //			break;
        //	}
        //	void ProcessChange<T>(Func<T> getter, Action<T> setter)
        //	{
        //		if (setter != null)
        //		{
        //			EditorGUI.BeginChangeCheck();
        //			var t = getter();
        //			if (EditorGUI.EndChangeCheck())
        //			{
        //				setter(t);
        //			}
        //		}
        //	}
        //}
        //private static void EditTransform(Transform transform, Color selectedColor, float size, Handles.CapFunction method, bool showGizmo, Action<Vector3> translate)
        //{
        //	EditTool(transform.position, transform.rotation, selectedColor, size, method, showGizmo, translate);
        //}
        //private static void EditTool(Vector3 position, Quaternion orientation, Color selectedColor, float size, Handles.CapFunction method, bool showGizmo, Action<Vector3> translate)
        //{
        //	if (Selection.activeTransform == transform)
        //	{
        //		DrawHandlesButton(selectedColor, transform.position, size, method);
        //		switch (Tools.current)
        //		{
        //			case Tool.Transform:
        //			case Tool.Move:
        //				if (type.HasEnumFlag(TransformType.Translate))
        //				{
        //					if (EditPosition(selectedColor, position, orientation, size, showGizmo, method, out var pos))
        //						translate(pos);
        //					EditorGUI.BeginChangeCheck();
        //					var pos = EditPosition(selectedColor, position, Quaternion.identity, size, showGizmo, method);
        //					if (EditorGUI.EndChangeCheck())
        //					{
        //						translate(pos);
        //						//Undo.RecordObject(transform, $"Move {transform.name}");
        //						//transform.position = pos;
        //					}
        //				}
        //				break;
        //		}

        //	}
        //	else if (DrawHandlesButton(unselectedColor, transform.position, size, method))
        //	{
        //		Selection.activeTransform = transform;
        //	}
        //}
        public static bool EditPosition(Vector3 position, Quaternion orientation, out Vector3 newPosition)
        {
            EditorGUI.BeginChangeCheck();
            newPosition = Handles.DoPositionHandle(position, Quaternion.Normalize(orientation));
            var result = EditorGUI.EndChangeCheck();
            return result;
        }
        public static bool EditRotation(Vector3 position, Quaternion rotation, out Quaternion newRotation)
        {
            EditorGUI.BeginChangeCheck();
            newRotation = Handles.DoRotationHandle(rotation, position);
            var result = EditorGUI.EndChangeCheck();
            return result;
        }
        public static bool EditScale(Vector3 position, Quaternion rotation, Vector3 scale, out Vector3 newScale)
        {
            EditorGUI.BeginChangeCheck();
            newScale = Handles.DoScaleHandle(scale, position, rotation, 1f);
            var result = EditorGUI.EndChangeCheck();
            return result;
        }
        public static bool PointEditPosition(Color color, Vector3 position, Quaternion orientation, float size, out Vector3 newPosition)
        {
            return EditPosition(color, position, orientation, size, false, Handles.DotHandleCap, out newPosition);
        }
        private static bool EditPosition(Color color, Vector3 position, Quaternion orientation, float size, bool showToolGizmo, Handles.CapFunction method, out Vector3 newPosition)
        {
            EditorGUI.BeginChangeCheck();
            if (method != null)
            {
                var move = GetNewCapPosition();
                newPosition = showToolGizmo ? GetToolPosition() : move;
            }
            else
            {
                newPosition = GetToolPosition();
            }
            var result = EditorGUI.EndChangeCheck();
            return result;
            Vector3 GetNewCapPosition()
            {
                return ApplyColor(color, () => Handles.FreeMoveHandle(position, GetConstantSize(size, position), Vector3.zero, method));
            }
            Vector3 GetToolPosition()
            {
                return Handles.PositionHandle(position, orientation);
            }
        }
        public static Vector2 DrawEditableCircle(Vector3 pos, float radius)
        {
            return Handles.FreeMoveHandle(pos,
                HandleUtility.GetHandleSize(pos) * radius,
                Vector3.zero,
                Handles.CircleHandleCap);
        }
        public static void DrawCameraAlignedCircle(Vector3 pos, float radius, bool constantSize)
        {
            DrawCircle(pos, SceneView.lastActiveSceneView.camera.transform.forward, radius, constantSize);
        }
        public static void DrawCircle(Vector3 pos, Vector3 normal, float radius, bool constantSize)
        {
            Handles.DrawWireDisc(pos, normal, GetSize(pos, radius, constantSize));
        }

        public static void DrawDot(Color color, Vector3 pos, float size, bool constantSize) => DrawHandle(color, pos, Quaternion.identity, size, constantSize, Handles.DotHandleCap);
        public static void DrawAABB(Color color, Vector2 p1, Vector2 p2, Vector2 center)
        {
            DrawAABB(color, p1 + center, p2 + center);
        }
        public static bool DrawClickablePoint(Vector3 position, float size, bool constantSize)
        {
            return DrawHandlesButton(position, Quaternion.identity, size, Handles.SphereHandleCap, constantSize);
        }
        //public static void DrawSelectablePoint(Color color, Color selectedColor, Vector3 position, float size, bool constantSize, Condition isSelected, Action onSelect, Action selectedUpdate)
        //{
        //	DrawSelectableHandle(color, selectedColor, position, Quaternion.identity, size, Handles.DotHandleCap, constantSize, isSelected, onSelect, selectedUpdate);
        //}
        //private static void DrawSelectableHandle(Color color, Color selectedColor, Vector3 position, Quaternion rotation, float size, Handles.CapFunction handle, bool constantSize, Condition isSelected, Action onSelect, Action selectedUpdate)
        //{
        //	var selected = isSelected();
        //	if (selected)
        //	{
        //		DrawHandle(selectedColor, position, rotation, size, handle, constantSize);
        //		selectedUpdate?.Invoke();
        //	}
        //	else if (DrawHandlesButton(color, position, rotation, size, handle, constantSize))
        //		onSelect.Invoke();
        //}
        private static void DrawHandle(Vector3 position, Quaternion rotation, float size, Handles.CapFunction handle, bool constantSize)
        {
            handle(0, position, rotation, GetSize(position, size, constantSize), EventType.Repaint);
        }
        private static bool DrawHandlesButton(Vector3 position, Quaternion rotation, float size, Handles.CapFunction handle, bool constantSize)
        {
            return Handles.Button(position, rotation, GetSize(position, size, constantSize), size, handle);
        }
        private static float GetSize(Vector3 position, float size, bool constantSize) => (constantSize ? HandleUtility.GetHandleSize(position) : 1f) * size;
        #region Editable
        public static void DrawEditableCircle(ref Vector2 pos, Vector2 center)
        {
            var point = pos + center;
            Vector2 result = DrawEditableCircle(point, 0.1f);//Handles.FreeMoveHandle(point, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.1f, Vector3.zero, Handles.CircleHandleCap);
            pos = result - center;
        }
        //public static void DrawEditablePosition(Color lineColor, Color centerColor, ref Vector2 pos, Vector2 center)
        //{
        //	DrawEditableCircle(centerColor, ref pos, center);
        //	DrawCross(lineColor, pos, center, 1, 1);
        //}
        #endregion
        public static void DrawCross(Color color, Vector2 pos, Vector2 center, float sizeX, float sizeY)
        {
            Vector3 p = center + pos;
            var x = Vector3.right * sizeX * 0.5f;
            var y = Vector3.up * sizeY * 0.5f;
            DrawLines(p + x, p - x);
            DrawLines(p + y, p - y);
        }
        private static float GetConstantSize(float size, Vector3 pos)
        {
            return HandleUtility.GetHandleSize(pos) * size;
        }
        private static Quaternion ToCapQuaternion(Vector3 dir)
        {
            return Quaternion.FromToRotation(Vector3.forward, dir.normalized);
        }
        public static void DrawArrow(Color color, Vector3 pos, Vector3 dir, float size, bool constantSize = true)
        {
            DrawArrow(color, pos, ToCapQuaternion(dir), size, constantSize);
        }
        public static void DrawArrow(Color color, Vector3 pos, Quaternion rotation, float size, bool constantSize = true)
        {
            var newSize = constantSize ? GetConstantSize(size, pos) : size;
            ApplyColor(color, () => Handles.ArrowHandleCap(0, pos, rotation, newSize, EventType.Repaint));
        }
        public static void DrawArrow(Color color, Vector3 from, Vector3 to)
        {
            var dir = to - from;
            DrawArrow(color, from, dir, dir.magnitude, false);
        }
        public static bool ArrowButton(Color color, Vector3 pos, Vector3 dir, float size)
        {
            return ArrowButton(color, pos, ToCapQuaternion(dir), size);
        }
        public static bool ArrowButton(Color color, Vector3 pos, Quaternion rotation, float size)
        {
            return Button(color, pos, rotation, size, Handles.ArrowHandleCap);
        }
        // public void LineCapFunction(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        // {
        //     if (eventType == EventType.Layout)
        //     {

        //     }
        //     else if (eventType == EventType.Repaint)
        //     {

        //     }
        // }
        private static bool IsHovering(int controlID, Event evt)
        {
            return controlID == HandleUtility.nearestControl && GUIUtility.hotControl == 0 && !(Tools.current == Tool.View);
        }
        private static void LineHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Layout:
                case EventType.MouseMove:
                    {
                        Vector3 direction = rotation * Vector3.forward;
                        var distanceToLine = HandleUtility.DistanceToPolyLine(position, position + direction * size);
                        HandleUtility.AddControl(controlID, distanceToLine);
                        break;
                    }
                case (EventType.Repaint):
                    {
                        Vector3 direction = rotation * Vector3.forward;
                        Handles.DrawLine(position, position + direction * size, Handles.lineThickness);
                        break;
                    }
            }
        }
        public static bool LineButton(Color color, Vector3 from, Vector3 to)
        {
            var distance = to - from;
            var dir = distance.normalized;
            Quaternion rotation;
            if (dir == Vector3.forward)
            {
                rotation = Quaternion.identity;
            }
            else if (dir == Vector3.back)
            {
                rotation = Quaternion.AngleAxis(180, Vector3.up);
            }
            else
            {
                var axis = Vector3.Cross(Vector3.forward, dir);
                var angle = Vector3.Angle(Vector3.forward, dir);
                rotation = Quaternion.AngleAxis(angle, axis);
            }
            var result = ApplyColor(color, () => Handles.Button(from, rotation, distance.magnitude, distance.magnitude, LineHandleCap));
            return result;
        }
        public static bool CircleButton(Color color, Vector3 pos, Quaternion direction, float size, bool constantSize = false)
        {
            return Button(color, pos, direction, size, Handles.CircleHandleCap, constantSize);
        }
        public static bool Button(Color color, Vector3 pos, Quaternion direction, float size, Handles.CapFunction displayFunc, bool constantSize = false)
        {
            var newSize = constantSize ? GetConstantSize(size, pos) : size;
            return ApplyColor(color, () => Handles.Button(pos, direction, newSize, newSize, displayFunc));
        }
        //public static void DrawLabel(string msg, Vector2 pos, Vector2 center)
        //{
        //    Handles.Label(pos + center, msg);
        //}
        public static void DrawText(Vector3 position, string txt)
        {
            Handles.Label(position, txt);
        }
        public static void DrawBox(Color faceColor, Color outlineColor, params Vector3[] points)
        {
            Handles.DrawSolidRectangleWithOutline(points, faceColor, outlineColor);
            // Handles.DrawLines(points,
            //                 new int[]
            //                 {
            //                     0,1,
            //                     1,2,
            //                     2,3,
            //                     3,0
            //                 });
        }
        public static void DrawBox(Color faceColor, Color outlineColor, Transform space, Vector2 center, Vector2 size)
        {
            var c = space.TransformPoint(center);
            var r = space.TransformVector(new Vector3(size.x, 0, 0));
            var u = space.TransformVector(new Vector3(0, size.y, 0));
            DrawBox(faceColor,
                    outlineColor,
                    c + r + u,
                    c + r - u,
                    c - r - u,
                    c - r + u);
        }
        public static void TranslateTransformGizmo(Vector3 pos, Quaternion localRot, Transform recordTransform, Action<Transform, Vector3> processChange)
        {
            if (Tools.current == Tool.Move)
            {
                var rot = Tools.pivotRotation == PivotRotation.Local ? localRot : Quaternion.identity;
                EditorGUI.BeginChangeCheck();
                var position = Handles.PositionHandle(pos, rot);
                if (EditorGUI.EndChangeCheck())
                {
                    if (recordTransform)
                        Undo.RecordObject(recordTransform, "Moved " + recordTransform.name);
                    processChange(recordTransform, position);
                }
            }
        }
        public static void RotateTransformGizmo(Vector3 pos, Quaternion rot, Transform recordTransform, Action<Transform, Quaternion> processChange)
        {
            if (Tools.current == Tool.Rotate)
            {
                EditorGUI.BeginChangeCheck();
                var rotation = Handles.RotationHandle(rot, pos);
                if (EditorGUI.EndChangeCheck())
                {
                    if (recordTransform)
                        Undo.RecordObject(recordTransform, "Rotated " + recordTransform.name);
                    processChange(recordTransform, rotation);
                }
            }
        }
        //public static void DrawBox(Color faceColor, Color outlineColor, BoxCollider box)
        //{
        //	Vector2 GetExtent(Vector3 dir)
        //	{
        //		var dot = Vector3.Dot(dir, box.size);
        //		var result = ((Vector2)dir * dot).Mul(box.transform.lossyScale);
        //		return result;
        //	}
        //	var x = GetExtent(box.transform.right);
        //	var y = GetExtent(box.transform.up);
        //	Vector2 pos = box.transform.position + box.center;
        //	var points = new Vector3[]
        //		{
        //		x +y+pos,
        //		x-y+pos,
        //		-x-y+pos,
        //		-x+y+pos
        //		};
        //	DrawBox(faceColor, outlineColor, points);
        //}
        public static bool LeftMouseDown()
        {
            return MouseDown(0);
        }
        public static bool RightMouseDown()
        {
            return MouseDown(1);
        }
        public static bool MiddleMouseDown()
        {
            return MouseDown(2);
        }
        private static bool MouseDown(int mouseId)
        {
            var e = Event.current;
            var result = e.type == EventType.MouseDown && e.button == mouseId;
            return result;
        }
        public static bool KeyDown(KeyCode key)
        {
            var e = Event.current;
            return e.type == EventType.KeyDown && e.keyCode == key;
        }
    }
}
#endif
