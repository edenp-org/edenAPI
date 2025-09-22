using System;

namespace WebApplication3.Foundation.Exceptions;

public class AuthenticationException(string message) : Exception(message);