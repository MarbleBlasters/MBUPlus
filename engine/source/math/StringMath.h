//-----------------------------------------------------------------------------
// StringExtension: Useful templated methods for converting strings into Torque
// data types and vice-versa.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Copyright(c) 2015 The Platinum Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//-----------------------------------------------------------------------------

#pragma once

#include <string.h>
#include "platform/types.h"
#include <stdio.h>
#include "math/mPoint.h"
#include "math/mQuat.h"
#include "math/mBox.h"
#include "core/color.h"
#include "ortho.h"

#ifdef _WIN32
#    define strcasecmp _stricmp
#else
// windows and its security checks
#    define sscanf_s sscanf
#    define sprintf_s snprintf
#endif

#include "stb_sprintf.h"

#undef min
#undef max

/**
 * String <-> Math conversion helpers. Templated for maximum efficiency.
 */
namespace StringMath {
	/**
		 * Scan a string and convert it to its respective type.
		 * @arg str The string to read from.
		 * @return The string's value in a type.
		 */
	template <typename T>
	T scan(const char* str);
	/**
		 * Print a type to a string.
		 * @arg val The value to write to a string.
		 * @return The string value for that value.
		 */
	template <typename T>
	const char* print(const T& val);

	/**
		 * Scan a bool to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a bool.
		 */
	template <>
	inline bool scan<bool>(const char* str) {
		if (strcasecmp(str, "false") == 0)
			return false;
		if (strcasecmp(str, "0") == 0)
			return false;
		if (*str == 0)
			return false;
		return true;
	}
	/**
		 * Scan a U8 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a U8.
		 */
	template <>
	inline U8 scan<U8>(const char* str) {
		return atoi(str);
	}
	/**
		 * Scan a S8 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a S8.
		 */
	template <>
	inline S8 scan<S8>(const char* str) {
		return atoi(str);
	}
	/**
		 * Scan a U16 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a U16.
		 */
	template <>
	inline U16 scan<U16>(const char* str) {
		return atoi(str);
	}
	/**
		 * Scan a S16 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a S16.
		 */
	template <>
	inline S16 scan<S16>(const char* str) {
		return atoi(str);
	}
	/**
		 * Scan a U32 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a U32.
		 */
	template <>
	inline U32 scan<U32>(const char* str) {
		return atoi(str);
	}
	/**
		* Scan a S32 to a string.
		* @arg str The string to read from.
		* @return The string's value as a S32.
		*/
	template <>
	inline S32 scan<S32>(const char* str) {
		return atoi(str);
	}
	/**
		 * Scan a U64 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a U64.
		 */
	template <>
	inline U64 scan<U64>(const char* str) {
		return atoll(str);
	}
	/**
		 * Scan a S64 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a S64.
		 */
	template <>
	inline S64 scan<S64>(const char* str) {
		return atoll(str);
	}
	/**
		 * Scan an F32 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as an F32.
		 */
	template <>
	inline F32 scan<F32>(const char* str) {
		return (F32)atof(str);  //Atof returns double
	}
	/**
		 * Scan an F64 to a string.
		 * @arg str The string to read from.
		 * @return The string's value as an F64.
		 */
	template <>
	inline F64 scan<F64>(const char* str) {
		return atof(str);
	}
	/**
		 * Scan a Point2I to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a Point2I.
		 */
	template <>
	inline Point2I scan<Point2I>(const char* str) {
		Point2I p;
		sscanf_s(str, "%d %d", &p.x, &p.y);
		return p;
	}
	/**
		 * Scan a Point2F to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a Point2F.
		 */
	template <>
	inline Point2F scan<Point2F>(const char* str) {
		Point2F p;
		sscanf_s(str, "%f %f", &p.x, &p.y);
		return p;
	}
	/**
		 * Scan a Point3F to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a Point3F.
		 */
	template <>
	inline Point3F scan<Point3F>(const char* str) {
		Point3F p;
		sscanf_s(str, "%f %f %f", &p.x, &p.y, &p.z);
		return p;
	}
	/**
		 * Scan a Point3D to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a Point3D.
		 */
	template <>
	inline Point3D scan<Point3D>(const char* str) {
		Point3D p;
		sscanf_s(str, "%lf %lf %lf", &p.x, &p.y, &p.z);
		return p;
	}
	/**
		 * Scan a QuatF to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a QuatF.
		 */
	template <>
	inline QuatF scan<QuatF>(const char* str) {
		QuatF q;
		sscanf_s(str, "%f %f %f %f", &q.w, &q.x, &q.y, &q.z);
		return q;
	}
	/**
		 * Scan an AngAxisF to a string.
		 * @arg str The string to read from.
		 * @return The string's value as an AngAxisF.
		 */
	template <>
	inline AngAxisF scan<AngAxisF>(const char* str) {
		AngAxisF a;
		sscanf_s(str, "%f %f %f %f", &a.axis.x, &a.axis.y, &a.axis.z, &a.angle);
		return a;
	}
	/**
		 * Scan a MatrixF to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a MatrixF.
		 */
	template <>
	inline MatrixF scan<MatrixF>(const char* str) {
		Point3F p;
		AngAxisF a;
		MatrixF mat;
		int count = sscanf_s(str, "%f %f %f %f %f %f %f", &p.x, &p.y, &p.z, &a.axis.x, &a.axis.y, &a.axis.z, &a.angle);
		if (count >= 3 && count < 7)
		{
			// Angle wasn't saved, reset it
			a.axis = Point3F(1, 0, 0);  // Default to X axis
			a.angle = 0.0f;  // Default to no rotation
		}
		a.setMatrix(&mat);
		mat.setPosition(p);
		return mat;
	}
	/**
		 * Scan an OrthoF to a string.
		 * @arg str The string to read from.
		 * @return The string's value as an OrthoF.
		 */
	template <>
	inline OrthoF scan<OrthoF>(const char* str) {
		OrthoF o;
		sscanf_s(str, "%f %f %f %f %f %f %f %f %f", &o.right.x, &o.right.y, &o.right.z, &o.back.x, &o.back.y, &o.back.z,
			&o.down.x, &o.down.y, &o.down.z);
		return o;
	}
	/**
		* Scan a Box3F to a string.
		* @arg str The string to read from.
		* @return The string's value as a Box3F.
		*/
	template <>
	inline Box3F scan<Box3F>(const char* str) {
		Box3F b;
		;
		sscanf_s(str, "%f %f %f %f %f %f", &b.min.x, &b.min.y, &b.min.z, &b.max.x,
			&b.max.y, &b.max.z);
		return b;
	}

	/**
		 * Print a bool to a string.
		 * @arg val The bool to write to a string.
		 * @return The string value for that bool.
		 */
	template <>
	inline const char* print<bool>(const bool& val) {
		return val ? "1" : "0";
	}
	/**
		 * Print a U8 to a string.
		 * @arg val The U8 to write to a string.
		 * @return The string value for that U8.
		 */
	template <>
	inline const char* print<U8>(const U8& val) {
		char* ret = Con::getReturnBuffer(8);
		// fmt::sprintf("%hhu", val);
		stbsp_snprintf(ret, 8, "%hhu", val);
		return ret;
	}
	/**
		* Print a S8 to a string.
		* @arg val The S8 to write to a string.
		* @return The string value for that S8.
		*/
	template <>
	inline const char* print<S8>(const S8& val) {
		char* ret = Con::getReturnBuffer(8);
		stbsp_snprintf(ret, 8, "%hhd", val);
		return ret;
	}
	/**
		 * Print a U16 to a string.
		 * @arg val The U16 to write to a string.
		 * @return The string value for that U16.
		 */
	template <>
	inline const char* print<U16>(const U16& val) {
		char* ret = Con::getReturnBuffer(8);
		stbsp_snprintf(ret, 8, "%hu", val);
		return ret;
	}
	/**
		* Print a S16 to a string.
		* @arg val The S16 to write to a string.
		* @return The string value for that S16.
		*/
	template <>
	inline const char* print<S16>(const S16& val) {
		char* ret = Con::getReturnBuffer(8);
		stbsp_snprintf(ret, 8, "%hd", val);
		return ret;
	}
	/**
		 * Print a U32 to a string.
		 * @arg val The U32 to write to a string.
		 * @return The string value for that U32.
		 */
	template <>
	inline const char* print<U32>(const U32& val) {
		char* ret = Con::getReturnBuffer(16);
		stbsp_snprintf(ret, 16, "%u", val);
		return ret;
	}
	/**
		* Print a S32 to a string.
		* @arg val The S32 to write to a string.
		* @return The string value for that S32.
		*/
	template <>
	inline const char* print<S32>(const S32& val) {
		char* ret = Con::getReturnBuffer(16);
		stbsp_snprintf(ret, 16, "%d", val);
		return ret;
	}
	/**
		 * Print a U64 to a string.
		 * @arg val The U64 to write to a string.
		 * @return The string value for that U64.
		 */
	template <>
	inline const char* print<U64>(const U64& val) {
		char* ret = Con::getReturnBuffer(32);
		stbsp_snprintf(ret, 32, "%llu", val);
		return ret;
	}
	/**
		 * Print a S64 to a string.
		 * @arg val The S64 to write to a string.
		 * @return The string value for that S64.
		 */
	template <>
	inline const char* print<S64>(const S64& val) {
		char* ret = Con::getReturnBuffer(32);
		stbsp_snprintf(ret, 32, "%lld", val);
		return ret;
	}
	/**
		 * Print an F32 to a string.
		 * @arg val The F32 to write to a string.
		 * @return The string value for that F32.
		 */
	template <>
	inline const char* print<F32>(const F32& val) {
		char* ret = Con::getReturnBuffer(16);
		stbsp_snprintf(ret, 16, "%.7g", val);  //Enough places to suffice
		return ret;
	}
	/**
		 * Print an F64 to a string.
		 * @arg val The F64 to write to a string.
		 * @return The string value for that F64.
		 */
	template <>
	inline const char* print<F64>(const F64& val) {
		char* ret = Con::getReturnBuffer(32);
		stbsp_snprintf(ret, 32, "%.7g", val);  //Using %f to eliminate scientific notation
		return ret;
	}
	/**
		 * Print a Point2I to a string.
		 * @arg val The Point2I to write to a string.
		 * @return The string value for that Point2I.
		 */
	template <>
	inline const char* print<Point2I>(const Point2I& val) {
		char* ret = Con::getReturnBuffer(32);
		stbsp_snprintf(ret, 32, "%d %d", val.x, val.y);
		return ret;
	}
	/**
		 * Print a Point2F to a string.
		 * @arg val The Point2F to write to a string.
		 * @return The string value for that Point2F.
		 */
	template <>
	inline const char* print<Point2F>(const Point2F& val) {
		char* ret = Con::getReturnBuffer(32);
		stbsp_snprintf(ret, 32, "%.7g %.7g", val.x, val.y);
		return ret;
	}
	/**
		 * Print a Point3F to a string.
		 * @arg val The Point3F to write to a string.
		 * @return The string value for that Point3F.
		 */
	template <>
	inline const char* print<Point3F>(const Point3F& val) {
		char* ret = Con::getReturnBuffer(64);
		stbsp_snprintf(ret, 64, "%.7g %.7g %.7g", val.x, val.y, val.z);
		return ret;
	}
	/**
		 * Print a Point3D to a string.
		 * @arg val The Point3D to write to a string.
		 * @return The string value for that Point3D.
		 */
	template <>
	inline const char* print<Point3D>(const Point3D& val) {
		char* ret = Con::getReturnBuffer(64);
		stbsp_snprintf(ret, 64, "%.7g %.7g %.7g", val.x, val.y, val.z);
		return ret;
	}
	/**
		 * Print a QuatF to a string.
		 * @arg val The QuatF to write to a string.
		 * @return The string value for that Point3F.
		 */
	template <>
	inline const char* print<QuatF>(const QuatF& val) {
		char* ret = Con::getReturnBuffer(64);
		stbsp_snprintf(ret, 64, "%.7g %.7g %.7g %.7g", val.w, val.x, val.y, val.z);
		return ret;
	}
	/**
		 * Print an AngAxisF to a string.
		 * @arg val The AngAxisF to write to a string.
		 * @return The string value for that AngAxisF.
		 */
	template <>
	inline const char* print<AngAxisF>(const AngAxisF& val) {
		char* ret = Con::getReturnBuffer(64);
		stbsp_snprintf(ret, 64, "%.7g %.7g %.7g %.7g", val.axis.x, val.axis.y, val.axis.z, val.angle);
		return ret;
	}
	/**
		 * Scan an ColorF to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a ColorF.
		 */
	template <>
	inline ColorF scan<ColorF>(const char* str) {
		ColorF c;
		sscanf_s(str, "%f %f %f %f", &c.red, &c.green, &c.blue, &c.alpha);
		//Hack because sometimes the alpha is missing
		c.alpha = 1.0;
		return c;
	}
	/**
		 * Scan an ColorI to a string.
		 * @arg str The string to read from.
		 * @return The string's value as a ColorI.
		 */
	template <>
	inline ColorI scan<ColorI>(const char* str) {
		ColorI c;
		sscanf_s(str, "%hhu %hhu %hhu %hhu", &c.red, &c.green, &c.blue, &c.alpha);
		//Hack because sometimes the alpha is missing
		c.alpha = 255;
		return c;
	}
	/**
		 * Print a MatrixF to a string.
		 * @arg val The MatrixF to write to a string.
		 * @return The string value for that MatrixF.
		 */
	template <>
	inline const char* print<MatrixF>(const MatrixF& val) {
		AngAxisF a(val);
		Point3F p = val.getPosition();

		char* ret = Con::getReturnBuffer(128);
		stbsp_snprintf(ret, 128, "%.7g %.7g %.7g %.7g %.7g %.7g %.7g", p.x, p.y, p.z, a.axis.x, a.axis.y, a.axis.z, a.angle);
		return ret;
	}
	/**
		 * Print an OrthoF to a string.
		 * @arg val The OrthoF to write to a string.
		 * @return The string value for that OrthoF.
		 */
	template <>
	inline const char* print<OrthoF>(const OrthoF& val) {
		char* ret = Con::getReturnBuffer(256);
		stbsp_snprintf(ret, 256, "%f %f %f %f %f %f %f %f %f", val.right.x, val.right.y, val.right.z, val.back.x, val.back.y,
			val.back.z, val.down.x, val.down.y, val.down.z);
		return ret;
	}
	/**
		* Print a Box3F to a string.
		* @arg val The Box3F to write to a string.
		* @return The string value for that Box3F.
		*/
	template <>
	inline const char* print<Box3F>(const Box3F& val) {
		char* ret = Con::getReturnBuffer(256);
		stbsp_snprintf(ret, 256, "%f %f %f %f %f %f", val.min.x, val.min.y, val.min.z, val.max.x,
			val.max.y, val.max.z);
		return ret;
	}
}  // namespace StringMath
