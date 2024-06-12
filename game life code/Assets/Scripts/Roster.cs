using System.Collections.Generic;

public class Roster {
    private List<byte[,]> roster;
    private int stepsInPast;
    private int nowFlag;
    private int oldestFlag;
    private const int size = 51;

    public Roster(byte[,] statement) {
        roster = new List<byte[,]>(size);
        roster.Add((byte[,]) statement.Clone());
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

        if (roster.Count == nowFlag && roster.Count < size) roster.Add((byte[,]) statement.Clone());
        else roster[nowFlag] = (byte[,]) statement.Clone();
    }

    private int NextFlag(int i = 1) {return (nowFlag+i) % size;}

    public byte[,] Undo() {
        if (stepsInPast >= size - 1) return null;
        if (NextFlag(size - stepsInPast -1) == oldestFlag) return null;
        
        stepsInPast++;
        return FieldInPast();
    }

    public byte[,] Redo() {
        if (stepsInPast == 0) return null;
        
        stepsInPast--;
        return FieldInPast();
    }

    private byte[,] FieldInPast() {
        int pastFlag = nowFlag - stepsInPast;
        if (pastFlag < 0) pastFlag += size;
        return (byte[,]) roster[pastFlag].Clone();
    }
}