﻿namespace Naninovel.Parsing;

/// <summary>
/// Implementation is able to handle associations between parsed line semantics
/// and script text line ranges used to represent them.
/// </summary>
public interface IAssociator
{
    /// <summary>
    /// Handles association between the provided component and range.
    /// </summary>
    void Associate (ILineComponent component, LineRange range);
}