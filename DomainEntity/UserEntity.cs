﻿using MX.Database.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DomainEntity.Entities;

public class UserEntity : BaseEntity<int>
{
    [MaxLength(200)]
    public string FirstName { get; set; }

    [MaxLength(200)]
    public string LastName { get; set; }

    [MaxLength(200)]
    public string Email { get; set; }

    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    private UserEntity()
    {

    }

    public UserEntity(string firstName, string lastName, string email, byte[] passwordHash, byte[] passwordSalt)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    [JsonIgnore]
    public ICollection<UserSettingsEntity> Settings { get; set; }

    /// <summary>
    ///  Users who follow the user
    /// </summary>
    [JsonIgnore]
    public ICollection<FollowerEntity> Followers { get; set; }

    /// <summary>
    /// Users who the user follows
    /// </summary>
    [JsonIgnore]
    public ICollection<FollowerEntity> Following { get; set; }

    [JsonIgnore]
    public ICollection<RestrictedUserEntity> RestrictedUsers { get; set; }
}
