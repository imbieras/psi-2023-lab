using Vogen;

namespace StudyBuddy.ValueObjects;

[ValueObject<Guid>]
public partial struct UserId {}

[ValueObject(typeof((double, double)))]
public partial struct Coordinates {}
