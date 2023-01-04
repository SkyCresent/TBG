using System;

namespace Map
{
    public class Point : IEquatable<Point>
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // .Equals(Object) ���·� �������� ���ϴ� �޼ҵ�
        // ���ϴ� �������� valueType�� ���� ���� boxing ������ �̷����� ������
        // IEquatable<T> , ���׸� Ÿ������ �����ϰ� �� ���Ŀ� �ʿ��� Ÿ�Կ� ���� �����ϸ� boxing, unboxing�� ���� �� �ִ�.

        //�ڵ� ����:
        public bool Equals(Point other)
        {
            // ������ ������ �ּҰ��� ���ϴ� object.ReferenceEquals
            // ���� �Ҵ�� ��ü�� ���� �ּ� ���� ���� '���� ����'�� �Ǵ��ϱ� ������, '�� ����'�� �ν��Ͻ��� ����ؼ��� �ȵ�.
            // �������� ������ ������ �ȳ����� false�� ��ȯ
            // ���� �ְԵǸ� �Ű����� Ÿ���� object�����̶� �ڽ̰����� ��ģ��.
            // �׷��� ���޸𸮿� ���� �Ҵ�ǰ� �ǰ�, ���ÿ� �ִ� ���� �ٸ� �ּҰ��� ����� ������ �׻� fals�� ��ȯ.
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            // checked : ����� ���� �����̳� ��ȯ�� ���� �����÷�, ����÷� �˻縦 �����Ϸ� �ܿ��� �ɷ��ֵ��� �ϴ� Ű����
            // unchecked : ����� ���� �����̳� ��ȯ�� ������ �����÷�, ����÷ΰ� ����� �ϴ��� "���� �ǵ��ѰŴϱ� �����ض�" �ϴ� Ű����
            // overflow : ������ Ÿ���� ǥ���� �� �ִ� �ִ밪�� �Ѿ���� �ּҰ��� �Ǵ� ���
            // underflow : ������ Ÿ���� ǥ���� �� �ִ� �ּҰ��� �Ѿ���� �ִ밪�� �Ǵ� ���
            // ^ ��Ʈ������ : �� �ǿ������� �����Ǵ� ��Ʈ�� ��Ÿ�� ����(XOR)�� ����
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}