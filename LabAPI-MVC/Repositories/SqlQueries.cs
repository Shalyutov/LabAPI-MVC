namespace LabAPI_MVC.Repositories;

public static class SqlQueries
{
    public const string GetReferralBase = """
                                              select r.referral_id,
                                                     r.patient_id,
                                                     r.issued,
                                                     r.weight,
                                                     r.height,
                                                     r.sex
                                                from prelab.referral r
                                               where r.referral_id = @id
                                          """;
    public const string GetReferralTests = """
                                              select rt.test_id,
                                                     t.name,
                                                     t.description,
                                                     b.biomaterial_id,
                                                     b.name,
                                                     b.description
                                                from prelab.referral_test rt
                                                join prelab.test t
                                                  on rt.test_id = t.test_id
                                                join prelab.biomaterial b
                                                  on b.biomaterial_id = t.biomaterial_id
                                               where rt.referral_id = @id
                                           """;

    public const string GetPatientBase = """
                                                 select p.patient_id,
                                                        p.full_name,
                                                        p.birth_date,
                                                        p.document,
                                                        p.phone,
                                                        p.email
                                                   from prelab.patient p
                                                  where p.patient_id = @id
                                         """;
    public const string GetReferralSamples = """
                                                select s.sample_id,
                                                       s.issued,
                                                       bc.bio_case_id,
                                                       bc.name,
                                                       bc.description,
                                                       bcn.bio_container_id,
                                                       bcn.name,
                                                       bcn.description,
                                                       b.biomaterial_id,
                                                       b.name,
                                                       b.description,
                                                       sp.supplier_id,
                                                       sp.name,
                                                       sp.description,
                                                       s.referral_id
                                                  from prelab.sample s
                                                  join prelab.bio_case bc
                                                    on bc.bio_case_id = s.bio_case_id
                                                  join prelab.bio_container bcn
                                                    on bcn.bio_container_id = bc.bio_container_id
                                                  join prelab.biomaterial b
                                                    on b.biomaterial_id = bcn.biomaterial_id
                                                  join prelab.supplier sp
                                                    on sp.supplier_id = bc.supplier_id
                                                 where s.referral_id = @id
                                             """;
    public const string GetSample = """
                                                select s.sample_id,
                                                       s.issued,
                                                       bc.bio_case_id,
                                                       bc.name,
                                                       bc.description,
                                                       bcn.bio_container_id,
                                                       bcn.name,
                                                       bcn.description,
                                                       b.biomaterial_id,
                                                       b.name,
                                                       b.description,
                                                       sp.supplier_id,
                                                       sp.name,
                                                       sp.description,
                                                       s.referral_id
                                                  from prelab.sample s
                                                  join prelab.bio_case bc
                                                    on bc.bio_case_id = s.bio_case_id
                                                  join prelab.bio_container bcn
                                                    on bcn.bio_container_id = bc.bio_container_id
                                                  join prelab.biomaterial b
                                                    on b.biomaterial_id = bcn.biomaterial_id
                                                  join prelab.supplier sp
                                                    on sp.supplier_id = bc.supplier_id
                                                 where s.sample_id = @id
                                             """;
    public const string IsPatientExists = """
                                             select count(1)
                                               from prelab.patient p
                                              where p.patient_id = @id
                                          """;
    public const string CreatePatient = """
                                            insert into prelab.referral (referral_id, patient_id, issued, weight, height, sex)
                                            values (@id, @patient, @issued, @weight, @height, @sex);
                                        """;
    public const string IsReferralExists= """
                                             select count(1)
                                               from prelab.referral r
                                              where r.referral_id = @id
                                          """;
    public const string SetPatientReferral = """
                                                update prelab.referral
                                                   set patient_id = @patient
                                                 where referral_id = @id
                                             """;
    public const string IsTestExists = """
                                           select count(1)
                                             from prelab.test t
                                            where t.test_id = @id
                                       """;
    public const string LinkReferralTest = """
                                           insert into prelab.referral_test (referral_id, test_id)
                                           values (@id, @test);
                                           """;
    public const string UnlinkReferralTest = """
                                             delete from prelab.referral_test
                                             where referral_id = @id and test_id = @test;
                                             """;
    public const string UpdateReferral = """
                                            update prelab.referral
                                               set weight = @weight,
                                                   height = @height,
                                                   sex = @sex
                                             where referral_id = @id;
                                         """;

    public const string CreateSample = """
                                          insert into prelab.sample (referral_id, sample_id, bio_case_id, issued)
                                          values (@referral, @sample, @bio_case_id, @issued);
                                       """;
    
    public const string DeleteSample = """
                                          delete from prelab.sample
                                          where sample_id = @sample;
                                       """;

    public const string IsBioCaseExists = """
                                              select count(1)
                                              from prelab.bio_case
                                              where bio_case_id = @bio_case_id;
                                          """;
    
}