// Bezier.cs
//
// Implementations for splines and paths with various degrees of smoothness. A 'path', or 'spline', is arbitrarily long
// and may be composed of smaller path sections called 'curves'. For example, a Bezier path is made from multiple
// Bezier curves.
//
// Regarding naming, the word 'spline' refers to any path that is composed of piecewise parts. Strictly speaking one
// could call a composite of multiple Bezier curves a 'Bezier Spline' but it is not a common term. In this file the
// word 'path' is used for a composite of Bezier curves.
//
// Copyright (c) 2006, 2017 Tristan Grimmer.
// Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby
// granted, provided that the above copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
// INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN
// AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
// PERFORMANCE OF THIS SOFTWARE.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


// A CubicBezierCurve represents a single segment of a Bezier path. It knows how to interpret 4 CVs using the Bezier basis
// functions. This class implements cubic Bezier curves -- not linear or quadratic.
public class CubicBezierCurve
{
	private Vector3[] controlVerts;

	public CubicBezierCurve(Vector3[] cvs)
	{
		SetControlVerts(cvs);
	}

	public void SetControlVerts(Vector3[] cvs)
	{
		// Cubic Bezier curves require 4 cvs.
		Assert.IsTrue(cvs.Length == 4);
		controlVerts = cvs;
	}

	public Vector3[] GetControlVerts()
	{
		// Cubic Bezier curves require 4 cvs.
		Assert.IsNotNull(controlVerts);
		return controlVerts;
	}

	public Vector3 GetPoint(float t)                            // t E [0, 1].
	{
		Assert.IsTrue((t >= 0.0f) && (t <= 1.0f));
		float c = 1.0f - t;

		// The Bernstein polynomials.
		float bb0 = c * c * c;
		float bb1 = 3 * t * c * c;
		float bb2 = 3 * t * t * c;
		float bb3 = t * t * t;

		Vector3 point = controlVerts[0] * bb0 + controlVerts[1] * bb1 + controlVerts[2] * bb2 + controlVerts[3] * bb3;
		return point;
	}

	public Vector3 GetTangent(float t)                          // t E [0, 1].
	{
		// See: http://bimixual.org/AnimationLibrary/beziertangents.html
		Assert.IsTrue((t >= 0.0f) && (t <= 1.0f));

		Vector3 q0 = controlVerts[0] + ((controlVerts[1] - controlVerts[0]) * t);
		Vector3 q1 = controlVerts[1] + ((controlVerts[2] - controlVerts[1]) * t);
		Vector3 q2 = controlVerts[2] + ((controlVerts[3] - controlVerts[2]) * t);

		Vector3 r0 = q0 + ((q1 - q0) * t);
		Vector3 r1 = q1 + ((q2 - q1) * t);
		Vector3 tangent = r1 - r0;
		return tangent;
	}

	public float GetClosestParam(Vector3 pos, float paramThreshold = 0.000001f)
	{
		return GetClosestParamRec(pos, 0.0f, 1.0f, paramThreshold);
	}

	float GetClosestParamRec(Vector3 pos, float beginT, float endT, float thresholdT)
	{
		float mid = (beginT + endT) / 2.0f;

		// Base case for recursion.
		if ((endT - beginT) < thresholdT)
			return mid;

		// The two halves have param range [start, mid] and [mid, end]. We decide which one to use by using a midpoint param calculation for each section.
		float paramA = (beginT + mid) / 2.0f;
		float paramB = (mid + endT) / 2.0f;

		Vector3 posA = GetPoint(paramA);
		Vector3 posB = GetPoint(paramB);
		float distASq = (posA - pos).sqrMagnitude;
		float distBSq = (posB - pos).sqrMagnitude;

		if (distASq < distBSq)
			endT = mid;
		else
			beginT = mid;

		// The (tail) recursive call.
		return GetClosestParamRec(pos, beginT, endT, thresholdT);
	}

	/// <summary>
	/// Divides the curve into (_iterations) line segments and returns their summed length
	/// </summary>
	/// <param name="_segments"> Amount of Segments</param>
	/// <returns></returns>
	public float GetLength(int _segments = 100)
	{
		float length = 0f; // init length
		float step = 1f / (float)_segments; // calculate the steplength

		for (float t = 0; t < 1f - step; t += step) // go trough the bezier curve step by step
        {
			length += Vector3.Distance(GetPoint(t), GetPoint(t + step)); // add the length of the current line segment to the length
		}

		return length;
	}
}