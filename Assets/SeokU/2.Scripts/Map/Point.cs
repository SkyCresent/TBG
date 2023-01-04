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

        // .Equals(Object) 형태로 동등한지 비교하는 메소드
        // 비교하는 과정에서 valueType의 값이 들어가면 boxing 현상이 이뤄지기 때문에
        // IEquatable<T> , 제네릭 타입으로 정의하고 난 이후에 필요한 타입에 따라 대응하면 boxing, unboxing을 피할 수 있다.

        //자동 생성:
        public bool Equals(Point other)
        {
            // 참조형 변수의 주소값을 비교하는 object.ReferenceEquals
            // 힙에 할당된 객체의 참조 주소 값을 비교해 '같음 여부'를 판단하기 때문에, '값 형식'의 인스턴스에 사용해서는 안됨.
            // 값형식을 넣으면 오류는 안나지만 false를 반환
            // 값을 넣게되면 매개변수 타입이 object형식이라 박싱과정을 거친다.
            // 그러면 힙메모리에 값이 할당되게 되고, 스택에 있는 서로 다른 주소값이 생기기 때문에 항상 fals를 반환.
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
            // checked : 상수값 들의 연산이나 변환에 의한 오버플로, 언더플로 검사를 컴파일러 단에서 걸러주도록 하는 키워드
            // unchecked : 상수값 들의 연산이나 변환에 으히ㅐ 오버플로, 언더플로가 생긴다 하더라도 "내가 의도한거니까 무시해라" 하는 키워드
            // overflow : 데이터 타입이 표현할 수 있는 최대값을 넘어버려 최소값이 되는 경우
            // underflow : 데이터 타입이 표현할 수 있는 최소값을 넘어버려 최대값이 되는 경우
            // ^ 비트연산자 : 두 피연산자의 대응되는 비트에 배타적 논리합(XOR)을 수행
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