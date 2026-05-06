//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

#ifndef _FILEOBJECT_H_
#define _FILEOBJECT_H_

#ifndef _SIMBASE_H_
#include "console/simBase.h"
#endif
#ifndef _RESMANAGER_H_
#include "core/resManager.h"
#endif
#ifndef _FILESTREAM_H_
#include "core/fileStream.h"
#endif

class FileObject : public SimObject
{
    typedef SimObject Parent;
    U8* mFileBuffer;
    U32 mBufferSize;
    U32 mCurPos;
    Stream* mStream;
public:
    FileObject();
    ~FileObject();

    bool openForWrite(const char* fileName, const bool append = false);
    bool openForRead(const char* fileName);
    bool readMemory(const char* fileName);
    const U8* buffer() { return mFileBuffer; }
    const U8* readLine();
    bool isEOF();
    void writeLine(const U8* line);
    void close();

    bool _read(U32 size, void* dst, U32* bytesRead = NULL) {
        //Torque just caches all of this in ram. Bad Torque!
        U32 position = mCurPos;

        //At least it makes this easy
        if (size + position > getBufferSize()) {
            size = getBufferSize() - position;
        }

        //Ugh, copy from memory to memory
        memcpy(dst, mFileBuffer + position, size);

        //*audible groan*
        mCurPos = position + size;
        if (bytesRead != NULL)
            *bytesRead = size;
        return true;
    }
    bool _write(U32 size, const void* src) {
        return mStream->_write(size, src);
    }

    inline U32 getCurPos() const {
        return mCurPos;
    }

    //Luma:  ccess to the buffer and the size
    U8 *getBuffer(void)     { return mFileBuffer; }
    U32 getBufferSize(void) { return mBufferSize; }

    DECLARE_CONOBJECT(FileObject);
};

#endif
