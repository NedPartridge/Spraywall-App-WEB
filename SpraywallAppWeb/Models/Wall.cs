﻿namespace SpraywallAppWeb.Models;

// Holds data related to 'walls' - for both management and viewing applications
public class Wall
{
    // Unique identifier: generated by EF when created
    public int Id { get; set; }

    // Wall name
    // Chosen by user, but manually kept unique for conveniance 
    public string Name { get; set; }

    // Paths to files
    // The data in the files is to big to be efficiently stored in the database,
    // so instead, they're kept under the webrootpath
    public string ImagePath { get; set; }
    public string IdentifiedHoldsJsonPath { get; set; }

    // Relationships: represent relation data between objects, for the EF
    public int ManagerID { get; set; }
    public User Manager { get; set; }
}
