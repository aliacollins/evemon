# region License
/* EVECacheParser - .NET 4/C# EVE Cache File Parser Library
 * Copyright © 2012 Jimi 'Desmont McCallock' C <jimikar@gmail.com>
 *
 * Based on:
 * - reverence - Python library for processing EVE Online cache and bulkdata
 *    Copyright © 2003-2011 Jamie 'Entity' van den Berge <jamie@hlekkir.com>
 *    https://github.com/ntt/reverence
 *
 * - libevecache - C++ EVE online reverse engineered cache reading library
 *    Copyright © 2009-2010  StackFoundry LLC and Yann 'Kaladr' Ramin <yann@stackfoundry.com>
 *    http://dev.eve-central.com/libevecache/
 *    http://gitorious.org/libevecache
 *    https://github.com/theatrus/libevecache
 *
 * - EveCache.Net - A port of libevecache to C#
 *    Copyright © 2011 Jason 'Jay Wareth' Watkins <jason@blacksunsystems.net>
 *    https://github.com/jwatkins42/EveCache.Net
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
# endregion

namespace EVEMon.MarketUnifiedUploader.EveCacheParser
{
    internal enum StreamType
    {
        None = 0x01,                // 1: None
        StringGlobal = 0x02,        // 2: A string identifying usually a type, function or class object
        Long = 0x3,                 // 3: 64 bit signed value
        Int = 0x04,                 // 4: 32 bit signed value
        Short = 0x05,               // 5: 16 bit signed value
        Byte = 0x6,                 // 6: 8 bit signed value
        IntNegOne = 0x07,           // 7: The value of -1
        IntZero = 0x08,             // 8: The value of 0
        IntOne = 0x09,              // 9: The value of 1
        Double = 0x0a,              // 10: 64 bit signed double
        DoubleZero = 0x0b,          // 11: The value of 0.0
        StringLong = 0x0d,          // 13: String, longer than 255 characters using normal count
        StringEmpty = 0xe,          // 14: String, empty
        StringOne = 0xf,            // 15: String, 1 character
        String = 0x10,              // 16: String, next byte is 0x00 - 0xff being the count
        StringRef = 0x11,           // 17: String, reference to line in StringsTable
        StringUnicode = 0x12,       // 18: String unicode, next byte is count
        StringIdent = 0x13,         // 19: Buffer object, identifier string
        Tuple = 0x014,              // 20: Tuple, next byte is count
        List = 0x15,                // 21: List, next byte is count
        Dict = 0x16,                // 22: Dictionary, next byte is count
        ClassObject = 0x017,        // 23: Class object, name of the class follows as string
        Blue = 0x18,                // 24: Blue object
        Callback = 0x19,            // 25: Callback
        SharedObj = 0x1b,           // 27: Shared object reference
        Checksum = 0x1c,            // 28: Checksum of rest of stream
        BoolTrue = 0x1f,            // 31: Boolean True
        BoolFalse = 0x20,           // 32: Boolean False
        Pickler = 0x21,             // 33: Standard pickle of undetermined size
        Object = 0x22,              // 34: Object
        NewObj = 0x23,              // 35: New object
        TupleEmpty = 0x24,          // 36: Tuple, empty
        TupleOne = 0x25,            // 37: Tuple, single element
        ListEmpty = 0x26,           // 38: List, empty
        ListOne = 0x27,             // 39: List, single element
        StringUnicodeEmpty = 0x28,  // 40: String unicode, empty
        StringUnicodeOne = 0x29,    // 41: String unicode, 1 character
        CompressedDBRow = 0x2a,     // 42: Database row, a RLEish compressed row
        SubStream = 0x2b,           // 43: Embedded stream (substream), next bytes after 'StreamStart' are length
        TupleTwo = 0x2c,            // 44: Tuple, two elements
        Marker = 0x2d,              // 45: Marker (for the NewObj/Object iterators that follow them)
        Utf8 = 0x2e,                // 46: UTF8 string unicode, next byte is buffer size count
        BigInt = 0x2f,              // 47: Big int, next byte is count

        SharedFlag = 0x40,          // 64: Flag for a shared object
        StreamStart = 0x7e          // 126: Start of each stream
    }

    internal enum DBTypes
    {
        Empty = 0,
        Null = 1,
        Short = 2,
        Int = 3,
        Float = 4,
        Double = 5,
        Currency = 6,
        Date = 7,
        BinaryString = 8,
        IDispatch = 9,
        Error = 10,
        Bool = 11,
        Variant = 12,
        BigInt = 13,
        Decimal = 14,
        Byte = 16,
        UByte = 17,
        UShort = 18,
        UInt = 19,
        Long = 20,
        ULong = 21,
        Filetime = 64,
        Guid = 72,
        Bytes = 128,
        String = 129,
        WideString = 130,
        Numeric = 131,
        UserDefinedType = 132,
        DBDate = 133,
        DBTime = 134,
        DBTimestamp = 135,
        HChapter = 136,
        DBLifetime = 137,
        PropVariant = 138,
        VarNumeric = 139,

        Vector = 0x1000,
        Array = 0x2000,
        ByRef = 0x4000,
        Reserved = 0x8000,
    }
}