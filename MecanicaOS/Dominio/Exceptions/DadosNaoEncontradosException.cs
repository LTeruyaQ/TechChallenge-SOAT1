﻿namespace Dominio.Exceptions;

public class DadosNaoEncontradosException : Exception
{
    public DadosNaoEncontradosException(string mensagem) : base(mensagem) { }
}

