﻿namespace Dominio.Exceptions;

public class DadosInvalidosException : Exception
{
    public DadosInvalidosException(string mensagem) : base(mensagem) { }
}

