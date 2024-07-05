using System.Collections.Generic;

public class Roster {
    private List<Action> roster;
    private int stepsInPast;
    private int nowFlag;
    private int oldestFlag;
    private const int size = 51;

    public Roster(byte[,] statement) {
        roster = new List<Action>(size);
        roster.Add(new Action2D((byte[,]) statement.Clone()));
        stepsInPast = 0;
        nowFlag = 0;
        oldestFlag = size-1;
    }

    public Roster(byte[,,] statement) {
        roster = new List<Action>(size);
        roster.Add(new Action3D((byte[,,]) statement.Clone()));
        stepsInPast = 0;
        nowFlag = 0;
        oldestFlag = size-1;
    }

    public void Add(byte[,] statement) {
        if (stepsInPast > 0) {
            nowFlag -= stepsInPast;
            if (nowFlag < 0) nowFlag += size;
            stepsInPast = 0;
        }
        else if (nowFlag == oldestFlag) oldestFlag = NextFlag();
        nowFlag = NextFlag();

        if (roster.Count == nowFlag && roster.Count < size) roster.Add(new Action2D((byte[,]) statement.Clone()));
        else roster[nowFlag] = new Action2D((byte[,]) statement.Clone());
    }

    public void Add(byte[,,] statement) {
        if (stepsInPast > 0) {
            nowFlag -= stepsInPast;
            if (nowFlag < 0) nowFlag += size;
            stepsInPast = 0;
        }
        else if (nowFlag == oldestFlag) oldestFlag = NextFlag();
        nowFlag = NextFlag();

        if (roster.Count == nowFlag && roster.Count < size) roster.Add(new Action3D((byte[,,]) statement.Clone()));
        else roster[nowFlag] = new Action3D((byte[,,]) statement.Clone());
    }

    private int NextFlag(int i = 1) {return (nowFlag+i) % size;}

    public byte[,] Undo2D() {
        if (stepsInPast >= size - 1) return null;
        if (NextFlag(size - stepsInPast -1) == oldestFlag) return null;
        
        stepsInPast++;
        return Field2DInPast();
    }

    public byte[,,] Undo3D() {
        if (stepsInPast >= size - 1) return null;
        if (NextFlag(size - stepsInPast -1) == oldestFlag) return null;
        
        stepsInPast++;
        return Field3DInPast();
    }

    public byte[,] Redo2D() {
        if (stepsInPast == 0) return null;
        
        stepsInPast--;
        return Field2DInPast();
    }

    public byte[,,] Redo3D() {
        if (stepsInPast == 0) return null;
        
        stepsInPast--;
        return Field3DInPast();
    }

    private byte[,] Field2DInPast() {
        int pastFlag = nowFlag - stepsInPast;
        if (pastFlag < 0) pastFlag += size;
        return (byte[,]) ((Action2D) roster[pastFlag]).statement.Clone();
    }

    private byte[,,] Field3DInPast() {
        int pastFlag = nowFlag - stepsInPast;
        if (pastFlag < 0) pastFlag += size;
        return (byte[,,]) ((Action3D) roster[pastFlag]).statement.Clone();
    }
}

internal abstract class Action {}

internal class Action2D : Action {
    internal byte[,] statement;
    internal Action2D(byte[,] statement) {this.statement = statement;}
}

internal class Action3D : Action {
    internal byte[,,] statement;
    internal Action3D(byte[,,] statement) {this.statement = statement;}
}