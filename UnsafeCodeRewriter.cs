using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using UnityEngine;

public class UnsafeCodeRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        try
        {
            Debug.Log($"Visiting ExpressionStatement: {node.ToFullString()}");

            if (node.Expression is InvocationExpressionSyntax invocation)
            {
                var expression = invocation.Expression.ToString();

                // Check for unsafe patterns
                if (UnsafePatterns.IsUnsafe(expression, out string replacement))
                {
                    Debug.Log($"Removing unsafe method call: {expression}");
                    return SyntaxFactory.ParseStatement(replacement + "\n");
                }
            }

            return base.VisitExpressionStatement(node);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during VisitExpressionStatement: {ex.Message}\n{ex.StackTrace}");
            return node;
        }
    }

    public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        try
        {
            Debug.Log($"Visiting ObjectCreationExpression: {node.ToFullString()}");

            var typeName = node.Type.ToString();

            // Check for unsafe patterns
            if (UnsafePatterns.IsUnsafe(typeName, out string replacement))
            {
                Debug.Log($"Removing unsafe object creation: {typeName}");
                return SyntaxFactory.ParseStatement(replacement + "\n");
            }

            return base.VisitObjectCreationExpression(node);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during VisitObjectCreationExpression: {ex.Message}\n{ex.StackTrace}");
            return node;
        }
    }

    public override SyntaxNode? VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        try
        {
            Debug.Log($"Visiting LocalDeclarationStatement: {node.ToFullString()}");

            // Check if the declaration has an initializer (EqualsValueClauseSyntax)
            foreach (var variable in node.Declaration.Variables)
            {
                if (variable.Initializer?.Value is InvocationExpressionSyntax invocation)
                {
                    var expression = invocation.Expression.ToString();

                    // Check for unsafe patterns
                    if (UnsafePatterns.IsUnsafe(expression, out string replacement))
                    {
                        Debug.Log($"Removing unsafe LocalDeclarationStatement: {node.ToFullString()}");
                        return null; // Remove the entire statement
                    }
                }
            }

            return base.VisitLocalDeclarationStatement(node);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during VisitLocalDeclarationStatement: {ex.Message}\n{ex.StackTrace}");
            return node;
        }
    }

    public override SyntaxNode VisitEqualsValueClause(EqualsValueClauseSyntax node)
    {
        try
        {
            Debug.Log($"Visiting EqualsValueClause: {node.ToFullString()}");

            // Check if the Value is an ObjectCreationExpression of type HttpClient
            if (node.Value is ObjectCreationExpressionSyntax objectCreation &&
                objectCreation.Type.ToString() == "HttpClient")
            {
                Debug.Log("Nullifying unsafe HttpClient assignment.");
                return SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression("null"));
            }

            return base.VisitEqualsValueClause(node);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during VisitEqualsValueClause: {ex.Message}\n{ex.StackTrace}");
            return node;
        }
    }


}
